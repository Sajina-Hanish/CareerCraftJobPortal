import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider, useAuth } from './auth/AuthContext';
import PrivateRoute from './auth/PrivateRoute';
import Navbar from './components/Navbar';

import LandingPage from './pages/LandingPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import EmployerDashboard from './pages/EmployerDashboard';
import JobSeekerDashboard from './pages/JobSeekerDashboard';
import ReceivedApplications from './pages/ReceivedApplications';
import MyApplications from './pages/MyApplications';
import ErrorPage from './pages/ErrorPage';

function AppRoutes() {
  const { user } = useAuth();

  return (
    <Routes>
      <Route path="/" element={<LandingPage />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      <Route path="/employer" element={
        <PrivateRoute role="Employer">
          <EmployerDashboard />
        </PrivateRoute>
      } />

      <Route path="/jobseeker" element={
        <PrivateRoute role="JobSeeker">
          <JobSeekerDashboard />
        </PrivateRoute>
      } />

      <Route path="/my-applications" element={
        <PrivateRoute role="JobSeeker">
          <MyApplications />
        </PrivateRoute>
      } />

      <Route path="/received-applications" element={
        <PrivateRoute role="Employer">
          <ReceivedApplications />
        </PrivateRoute>
      } />

      <Route path="*" element={<ErrorPage />} />
    </Routes>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Navbar />
        <AppRoutes />
      </BrowserRouter>
    </AuthProvider>
  );
}
