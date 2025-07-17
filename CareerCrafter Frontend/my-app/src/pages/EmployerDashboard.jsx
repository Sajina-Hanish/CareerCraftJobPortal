import { useEffect, useState } from 'react';
import { createJob, getEmployerJobs, apiFetch } from '../services/api';
import './EmployerDashboard.css';

export default function EmployerDashboard() {
  const [jobs, setJobs] = useState([]);
  const [form, setForm] = useState({
    title: '',
    description: '',
    location: '',
    company: '',
    qualification: '',
    salary: ''
  });

  const [errors, setErrors] = useState({});
  const [isEditing, setIsEditing] = useState(false);
  const [editId, setEditId] = useState(null);

  useEffect(() => {
    fetchJobs();
  }, []);

  const fetchJobs = async () => {
    try {
      const res = await getEmployerJobs();
      setJobs(res);
    } catch (err) {
      alert(err.message);
    }
  };

  const handleChange = e => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setErrors({ ...errors, [e.target.name]: '' });
  };

  const validate = () => {
    const newErrors = {};
    for (const key in form) {
      if (!form[key]) {
        newErrors[key] = `${key.charAt(0).toUpperCase() + key.slice(1)} is required`;
      }
    }
    return newErrors;
  };

  const handleSubmit = async e => {
    e.preventDefault();
    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {
      if (isEditing) {
        await apiFetch(`jobs/${editId}`, 'PUT', form, true);
        alert("Job updated!");
      } else {
        await createJob(form);
        alert("Job posted!");
      }
      resetForm();
      fetchJobs();
    } catch (err) {
      alert(err.message);
    }
  };

  const handleEdit = job => {
    setForm({
      title: job.title,
      description: job.description,
      location: job.location,
      company: job.company,
      qualification: job.qualification,
      salary: job.salary
    });
    setIsEditing(true);
    setEditId(job.id);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleDelete = async id => {
    if (!window.confirm("Are you sure to delete this job?")) return;
    try {
      await apiFetch(`jobs/${id}`, 'DELETE', null, true);
      fetchJobs();
    } catch (err) {
      alert(err.message);
    }
  };

  const resetForm = () => {
    setForm({ title: '', description: '', location: '', company: '', qualification: '', salary: '' });
    setIsEditing(false);
    setEditId(null);
    setErrors({});
  };

  return (
    <div className="employer-dashboard">
      <div className="overlay">
        <div className="container py-5">
          <h3 className="text-white mb-4">Your Posted Jobs</h3>
          <ul className="list-group mb-5">
            {jobs.map(job => (
              <li key={job.id} className="list-group-item d-flex justify-content-between align-items-center">
                <div>
                  <strong>{job.title}</strong> at {job.company} – {job.location} – ₹{job.salary}
                  <div><em>Qualification:</em> {job.qualification}</div>
                </div>
                <div>
                  <button className="btn edit-btn me-2" onClick={() => handleEdit(job)}>Edit</button>
                  <button className="btn delete-btn" onClick={() => handleDelete(job.id)}>Delete</button>
                </div>
              </li>
            ))}
          </ul>

          <div className="form-container bg-white p-4 rounded shadow">
            <h4 className="text-center mb-4">{isEditing ? "Edit Job" : "Post a New Job"}</h4>
            <form onSubmit={handleSubmit} className="row g-3">
              {["title", "description", "location", "company", "qualification", "salary"].map(field => (
                <div className="col-md-6" key={field}>
                  <input
                    className={`form-control ${errors[field] ? 'is-invalid' : ''}`}
                    name={field}
                    placeholder={field.charAt(0).toUpperCase() + field.slice(1)}
                    value={form[field]}
                    onChange={handleChange}
                  />
                  {errors[field] && <div className="invalid-feedback">{errors[field]}</div>}
                </div>
              ))}
              <div className="col-12 text-center">
                <button className="btn post-btn me-2">{isEditing ? "Update" : "Post Job"}</button>
                {isEditing && (
                  <button
                    type="button"
                    className="btn btn-secondary"
                    onClick={resetForm}
                  >
                    Cancel
                  </button>
                )}
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
