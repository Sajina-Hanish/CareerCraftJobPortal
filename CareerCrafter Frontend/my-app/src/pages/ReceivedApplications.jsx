import { useEffect, useState } from 'react';
import { getApplicationsForEmployer, API } from '../services/api';
import './ReceivedApplications.css';

export default function ReceivedApplications() {
  const [apps, setApps] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchApplications();
  }, []);

  async function fetchApplications() {
    try {
      const res = await getApplicationsForEmployer();
      setApps(res);
    } catch (err) {
      alert(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="received-applications-page">
      <div className="container py-5 d-flex justify-content-center">
        <div className="col-lg-8">
          <h4 className="fw-bold text-center mb-4">Applications for Your Job Posts</h4>
          {loading ? (
            <p className="text-center">Loading...</p>
          ) : apps.length === 0 ? (
            <p className="text-center">No applications received yet.</p>
          ) : (
            <ul className="list-group">
              {apps.map(app => (
                <li key={app.jobApplicationId} className="list-group-item mb-3 shadow-sm">
                  <strong>{app.job?.title}</strong> at {app.job?.company}<br />
                  Resume: <a
                    href={`${API.replace("/api", "")}/resumes/${app.resumePath}`}
                    target="_blank"
                    rel="noopener noreferrer"
                    download
                  >
                    Download Resume
                  </a><br />
                  Applicant Email: {app.jobSeeker?.email}<br />
                  Applied On: {new Date(app.appliedOn).toLocaleString()}
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
}
