import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import './Navbar.css';

export default function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const goToDashboard = () => {
    if (user?.role === 'Employer') navigate('/employer');
    else if (user?.role === 'JobSeeker') navigate('/jobseeker');
  };

  return (
    <nav className="navbar navbar-expand-lg custom-navbar px-3">
      <div className="container-fluid d-flex justify-content-between align-items-center w-100">
        {/* Left: Brand */}
        <Link className="navbar-brand fw-bold" to="/">CareerCrafter</Link>

        {/* Center: Role-based Buttons */}
        <div className="d-flex justify-content-center flex-grow-1">
          {user && (
            <>
              <button onClick={goToDashboard} className="btn btn-outline-light me-2">
                Dashboard
              </button>

              {user.role === 'JobSeeker' && (
                <Link to="/my-applications" className="btn btn-outline-light me-2">
                  My Applications
                </Link>
              )}

              {user.role === 'Employer' && (
                <Link to="/received-applications" className="btn btn-outline-light me-2">
                  Applications
                </Link>
              )}
            </>
          )}
        </div>

        {/* Right: User & Logout/Login */}
        <div className="d-flex align-items-center">
          {user ? (
            <>
              <span className="user-role me-3">{user.role}</span>
              <button onClick={logout} className="btn btn-outline-light btn-sm">Logout</button>
            </>
          ) : (
            <>
              <Link to="/login" className="btn btn-outline-light me-2">Login</Link>
              <Link to="/register" className="btn btn-outline-light">Register</Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
