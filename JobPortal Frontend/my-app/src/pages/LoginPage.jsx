import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { login as loginApi } from '../services/api';
import './LoginPage.css';
import bg from '../assets/LandingPageBackground.jpeg';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errors, setErrors] = useState({});
  const { login } = useAuth();
  const navigate = useNavigate();

  async function handleSubmit(e) {
    e.preventDefault();
    const newErrors = {};
    if (!email.trim()) {
      newErrors.email = "Email is required.";
    } else if (!email.endsWith("@gmail.com")) {
      newErrors.email = "Enter valid email";
    }
    if (!password.trim()) {
      newErrors.password = "Password is required.";
    }

    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    try {
      const res = await loginApi({ email, password });
      const payload = JSON.parse(atob(res.token.split('.')[1]));
      const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

      login(res.token, role);

      if (role === 'Employer') navigate('/employer');
      else if (role === 'JobSeeker') navigate('/jobseeker');
      else navigate('/');
    } catch (err) {
      setErrors({ general: "Invalid email or password." });
    }
  }


  return (
    <div className="login-page" style={{ backgroundImage: `url(${bg})` }}>
      <div className="card login-card">
        <h2 className="text-center mb-3">Login</h2>
        <form onSubmit={handleSubmit} noValidate>
          <input
            className="form-control my-2"
            type="email"
            placeholder="Email"
            value={email}
            onChange={e => {
              setEmail(e.target.value);
              setErrors(prev => ({ ...prev, email: null, general: null }));
            }}
          />
          {errors.email && <div className="text-danger mb-2">{errors.email}</div>}

          <input
            className="form-control my-2"
            type="password"
            placeholder="Password"
            value={password}
            onChange={e => {
              setPassword(e.target.value);
              setErrors(prev => ({ ...prev, password: null, general: null }));
            }}
          />
          {errors.password && <div className="text-danger mb-2">{errors.password}</div>}

          {errors.general && <div className="text-danger mb-3 text-center">{errors.general}</div>}

          <div className="text-center mt-3">
            <button className="btn text-white w-50" style={{ backgroundColor: '#02122e' }}>
              Login
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
