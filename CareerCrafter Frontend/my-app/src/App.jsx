import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider, useAuth } from './auth/AuthContext';
import ProtectedRoute from './auth/ProtectedRoute';
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
        <ProtectedRoute role="Employer">
          <EmployerDashboard />
        </ProtectedRoute>
      } />

      <Route path="/jobseeker" element={
        <ProtectedRoute role="JobSeeker">
          <JobSeekerDashboard />
        </ProtectedRoute>
      } />

      <Route path="/my-applications" element={
        <ProtectedRoute role="JobSeeker">
          <MyApplications />
        </ProtectedRoute>
      } />

      <Route path="/received-applications" element={
        <ProtectedRoute role="Employer">
          <ReceivedApplications />
        </ProtectedRoute>
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
