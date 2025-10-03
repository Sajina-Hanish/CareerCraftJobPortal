export const API = "https://localhost:7289/api";

export async function apiFetch(endpoint, method = 'GET', body = null, auth = false, isForm = false) {
  const headers = {};
  if (!isForm) headers['Content-Type'] = 'application/json';
    if (auth) {
    const token = localStorage.getItem('token');
    if (token) headers['Authorization'] = `Bearer ${token}`;
  }

  const res = await fetch(`${API}/${endpoint}`, {
    method,
    headers,
    body: body && !isForm ? JSON.stringify(body) : body,
  });

  const contentType = res.headers.get("content-type");
  if (!res.ok) {
    let errMsg = "API Error";
    if (contentType && contentType.includes("application/json")) {
      const errData = await res.json();
      errMsg = errData.message || errMsg;
    } else {
      errMsg = await res.text();
    }
    throw new Error(errMsg);
  }

  if (res.status === 204 || !contentType || !contentType.includes("application/json")) return;
  return await res.json();
}

export const login = (data) => apiFetch('auth/login', 'POST', data);
export const register = (data) => apiFetch('auth/register', 'POST', data);

export const createJob = (data) => apiFetch('jobs', 'POST', data, true);
export const getEmployerJobs = () => apiFetch('jobs/employer', 'GET', null, true);
export const getApplicationsForEmployer = () =>
  apiFetch('applications/employer', 'GET', null, true);

export const getAllJobs = () => apiFetch('jobs', 'GET', null, true);
export const searchJobs = (title, location) =>
  apiFetch(`jobs/search?title=${title}&location=${location}`, 'GET', null, true);
export const applyToJob = (jobId, resumeFile) => {
  const formData = new FormData();
  formData.append('jobId', jobId);
  formData.append('resume', resumeFile);
  return apiFetch('applications', 'POST', formData, true, true);
};

export const getMyApplications = () =>
  apiFetch('applications/my', 'GET', null, true);

export const deleteApplication = (id) =>
  apiFetch(`applications/${id}`, 'DELETE', null, true);

