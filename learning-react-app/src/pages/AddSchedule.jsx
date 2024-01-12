import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box,
  Typography,
  TextField,
  Button,
  Grid,
  InputLabel,
  Select,
  MenuItem,
} from "@mui/material";
import { useFormik } from "formik";
import * as yup from "yup";
import http from "../http";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { DatePicker, TimePicker } from "@mui/x-date-pickers";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import dayjs from "dayjs";
import utc from "dayjs/plugin/utc";
import timezone from "dayjs/plugin/timezone";
import { DateRangePicker } from "@mui/x-date-pickers-pro";
import FormControl from "@mui/material/FormControl";

function AddSchedule() {
  dayjs.extend(utc);
  dayjs.extend(timezone);

  const navigate = useNavigate();
  const [imageFile, setImageFile] = useState(null);

  const formik = useFormik({
    initialValues: {
      title: "",
      description: "",
      selectedDate: dayjs(),
      selectedTime: dayjs(),
    },
    validationSchema: yup.object({
      title: yup
        .string()
        .trim()
        .min(6, "Title must be at least 6 characters")
        .max(100, "Title must be at most 100 characters")
        .required("Title is required"),
      description: yup
        .string()
        .trim()
        .min(6, "Description must be at least 6 characters")
        .max(100, "Description must be at most 100 characters")
        .required("Description is required"),
    }),
    onSubmit: (data) => {
      // if (imageFile) {
      //   data.imageFile = imageFile;
      // }
      if(imageFile === null){
        return
      }

      data.imageFile = imageFile;
      data.title = data.title.trim();
      data.description = data.description.trim();
      const selectedDateTime = dayjs.tz(
        `${data.selectedDate.format("YYYY-MM-DD")} ${data.selectedTime.format(
          "HH:mm:ss"
        )}`,
        "Asia/Singapore"
      );

      data.selectedDate = selectedDateTime.format();
      data.selectedTime = selectedDateTime.format();

      http.post("/schedule", data).then((res) => {
        console.log(res.data);
        navigate("/schedules");
      });
    },
  });
  const onFileChange = (e) => {
    let file = e.target.files[0];
    if (file) {
      if (file.size > 1024 * 1024) {
        toast.error("Maximum file size is 1MB");
        return;
      }

      let formData = new FormData();
      formData.append("file", file);
      http
        .post("/file/upload", formData, {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        })
        .then((res) => {
          setImageFile(res.data.filename);
        })
        .catch(function (error) {
          console.log(error.response);
        });
    }
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <Box>
        <Typography variant="h5" sx={{ my: 2 }}>
          Add Schedule
        </Typography>
        <Box component="form" onSubmit={formik.handleSubmit}>
          <Grid container spacing={2}>
            <Grid item xs={12} md={6} lg={8}>
              <TextField
                fullWidth
                margin="dense"
                autoComplete="off"
                label="Title"
                name="title"
                value={formik.values.title}
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                error={formik.touched.title && Boolean(formik.errors.title)}
                helperText={formik.touched.title && formik.errors.title}
              />
              <TextField
                fullWidth
                margin="dense"
                autoComplete="off"
                multiline
                minRows={2}
                label="Description"
                name="description"
                value={formik.values.description}
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                error={
                  formik.touched.description &&
                  Boolean(formik.errors.description)
                }
                helperText={
                  formik.touched.description && formik.errors.description
                }
              />
              <DatePicker
                label="Please Choose A Date"
                defaultValue={formik.values.selectedDate}
                minDate={dayjs()}
                onChange={(newDate) =>
                  formik.setFieldValue("selectedDate", newDate)
                }
                error={
                  formik.touched.selectedDate &&
                  Boolean(formik.errors.selectedDate)
                }
                helperText={
                  formik.touched.selectedDate && formik.errors.selectedDate
                }
                slotProps={{
                  textField: {
                    disabled: true,
                  },
                }}
                timezone="Asia/Singapore"
                required
              />
              <TimePicker
                label="Please Choose A Time"
                defaultValue={formik.values.selectedTime}
                onChange={(newTime) =>
                  formik.setFieldValue("selectedTime", newTime)
                }
                error={
                  formik.touched.selectedTime &&
                  Boolean(formik.errors.selectedTime)
                }
                helperText={
                  formik.touched.selectedTime && formik.errors.selectedTime
                }
                slotProps={{
                  textField: {
                    disabled: true,
                  },
                }}
                timezone="Asia/Singapore"
                required
              />
              <Box sx={{ minWidth: 120 }}>
                <FormControl fullWidth>
                  <InputLabel id="demo-simple-select-label">
                    Available Places
                  </InputLabel>
                  <Select
                    labelId="demo-simple-select-label"
                    id="demo-simple-select"
                    label="Age"
                  >
                    <MenuItem value={"jg"}>Jurong</MenuItem>
                    <MenuItem value={"bl"}>Boon Lay</MenuItem>
                    <MenuItem value={"cck"}>Choa Chu Kang</MenuItem>
                  </Select>
                </FormControl>
              </Box>
            </Grid>
            <Grid item xs={12} md={6} lg={4}>
              <Box sx={{ textAlign: "center", mt: 2 }}>
                <Button variant="contained" component="label">
                  Upload Image
                  <input
                    hidden
                    accept="image/*"
                    multiple
                    type="file"
                    onChange={onFileChange}
                  />
                </Button>
                <Typography color="error" sx={{ mt: 1 }}>
                  Image is required
                </Typography>
                {imageFile && (
                  <Box className="aspect-ratio-container" sx={{ mt: 2 }}>
                    <img
                      alt="schedule"
                      src={`${import.meta.env.VITE_FILE_BASE_URL}${imageFile}`}
                    ></img>
                  </Box>
                )}
              </Box>
            </Grid>
          </Grid>
          <Box sx={{ mt: 2 }}>
            <Button variant="contained" type="submit">
              Add
            </Button>
          </Box>
        </Box>

        <ToastContainer />
      </Box>
    </LocalizationProvider>
  );
}

export default AddSchedule;
