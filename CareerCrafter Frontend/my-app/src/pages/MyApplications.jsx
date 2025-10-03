import { useEffect, useState } from 'react';
import { getMyApplications, deleteApplication, API } from '../services/api';
import './MyApplications.css';

export default function MyApplications() {
  const [apps, setApps] = useState([]);

  useEffect(() => {
    fetchApplications();
  }, []);

  async function fetchApplications() {
    try {
      const res = await getMyApplications();
      setApps(res);
    } catch (err) {
      alert(err.message);
    }
  }

  async function handleDelete(jobApplicationId) {
    if (!window.confirm("Are you sure you want to delete this application?")) return;
    try {
      await deleteApplication(jobApplicationId);
      setApps(prev => prev.filter(app => app.jobApplicationId !== jobApplicationId));
    } catch (err) {
      alert(err.message);
    }
  }

  return (
    <div className="my-applications-page">
      <div className="container d-flex justify-content-center py-5">
        <div className="col-lg-8">
          <h4 className="text-center mb-4 fw-bold">My Applications</h4>
          {apps.length === 0 ? (
            <p className="text-center">No applications yet.</p>
          ) : (
            <ul className="list-group">
              {apps.map(app => (
                <li key={app.jobApplicationId} className="list-group-item d-flex justify-content-between align-items-center mb-3 shadow-sm">
                  <div>
                    <strong>{app.job?.title}</strong> at {app.job?.company} – {app.job?.location}<br />
                    Applied On: {new Date(app.appliedOn).toLocaleDateString()}<br />
                    Resume: <a
                      href={`${API.replace("/api", "")}/resumes/${app.resumePath}`}
                      target="_blank"
                      rel="noopener noreferrer"
                    >View</a>
                  </div>
                  <button className="btn btn-sm btn-danger" onClick={() => handleDelete(app.jobApplicationId)}>Delete</button>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
}
