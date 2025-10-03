import { useEffect, useState } from 'react';
import { getAllJobs, applyToJob, searchJobs } from '../services/api';
import './JobSeekerDashboard.css';

export default function JobSeekerDashboard() {
  const [jobs, setJobs] = useState([]);
  const [resumeMap, setResumeMap] = useState({});
  const [errors, setErrors] = useState({});
  const [search, setSearch] = useState({ title: '', location: '' });
  const [searchErrors, setSearchErrors] = useState({});

  useEffect(() => {
    fetchJobs();
  }, []);

  const fetchJobs = async () => {
    try {
      const res = await getAllJobs();
      setJobs(res);
    } catch (err) {
      alert(err.message);
    }
  };

  const handleResumeChange = (jobId, file) => {
    setResumeMap(prev => ({ ...prev, [jobId]: file }));
    setErrors(prev => ({ ...prev, [jobId]: '' }));
  };

  const handleApply = async (jobId) => {
    const resumeFile = resumeMap[jobId];

    if (!resumeFile) {
      setErrors(prev => ({ ...prev, [jobId]: 'Please upload your resume before applying.' }));
      return;
    }

    if (resumeFile.type !== 'application/pdf') {
      setErrors(prev => ({ ...prev, [jobId]: 'Only PDF files are allowed.' }));
      return;
    }

    try {
      await applyToJob(jobId, resumeFile);
      alert("Applied successfully!");
      setErrors(prev => ({ ...prev, [jobId]: '' }));
    } catch (err) {
      alert(err.message);
    }
  };

  const handleSearch = async () => {
    const errs = {};
    if (!search.title.trim()) errs.title = 'Job Title is required';
    if (!search.location.trim()) errs.location = 'Location is required';

    setSearchErrors(errs);
    if (Object.keys(errs).length > 0) return;

    try {
      const res = await searchJobs(search.title, search.location);
      setJobs(res);
    } catch (err) {
      alert(err.message);
    }
  };

  return (
    <div className="jobseeker-dashboard">
      <div className="overlay">
        <div className="container py-5 d-flex justify-content-center">
          <div className="col-lg-10">
            <h3 className="text-white text-center mb-4">Available Jobs</h3>

            <div className="row mb-4 justify-content-center">
              <div className="col-md-4 mb-2">
                <input
                  className={`form-control ${searchErrors.title ? 'is-invalid' : ''}`}
                  placeholder="Job Title"
                  value={search.title}
                  onChange={e => {
                    setSearch({ ...search, title: e.target.value });
                    setSearchErrors(prev => ({ ...prev, title: '' }));
                  }}
                />
                {searchErrors.title && <div className="text-danger">{searchErrors.title}</div>}
              </div>

              <div className="col-md-4 mb-2">
                <input
                  className={`form-control ${searchErrors.location ? 'is-invalid' : ''}`}
                  placeholder="Location"
                  value={search.location}
                  onChange={e => {
                    setSearch({ ...search, location: e.target.value });
                    setSearchErrors(prev => ({ ...prev, location: '' }));
                  }}
                />
                {searchErrors.location && <div className="text-danger">{searchErrors.location}</div>}
              </div>

              <div className="col-md-2 mb-2">
                <button className="btn search-btn w-100" onClick={handleSearch}>Search</button>
              </div>
            </div>

            {jobs.map(job => (
              <div className="card mb-4" key={job.jobId}>
                <div className="card-body">
                  <h5 className="card-title">{job.title} – {job.company}</h5>
                  <p className="card-text">{job.description}</p>
                  <p><strong>Location:</strong> {job.location} | <strong>Salary:</strong> ₹{job.salary}</p>

                  <input
                    type="file"
                    accept=".pdf"
                    className={`form-control my-2 ${errors[job.jobId] ? 'is-invalid' : ''}`}
                    onChange={e => handleResumeChange(job.jobId, e.target.files[0])}
                  />
                  {errors[job.jobId] && <div className="text-danger">{errors[job.jobId]}</div>}

                  <button className="btn btn-success mt-2" onClick={() => handleApply(job.jobId)}>Apply</button>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
