import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { register } from '../services/api';
import './RegisterPage.css';
import bg from '../assets/LandingPageBackground.jpeg';

export default function RegisterPage() {
  const [form, setForm] = useState({ email: '', password: '', role: 'JobSeeker' });
  const [errors, setErrors] = useState({});
  const navigate = useNavigate();

  const validate = () => {
    const err = {};
    if (!form.email) {
      err.email = 'Email is required';
    } else if (!form.email.endsWith('@gmail.com')) {
      err.email = 'Email must be a valid';
    }

    if (!form.password) {
      err.password = 'Password is required';
    } else if (form.password.length < 6) {
      err.password = 'Password is too short (min. length: 6)';
    }

    return err;
  };

  const handleChange = e => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setErrors({ ...errors, [e.target.name]: '' }); 
  };

  async function handleSubmit(e) {
    e.preventDefault();
    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {
      await register(form);
      alert('Registration successful. Please login.');
      navigate('/login');
    } catch (err) {
      alert(err.message);
    }
  }

  return (
    <div className="register-page" style={{ backgroundImage: `url(${bg})` }}>
      <div className="card register-card">
        <h2 className="text-center mb-3">Register</h2>
        <form onSubmit={handleSubmit}>
          <input
            className="form-control my-2"
            name="email"
            placeholder="Email"
            value={form.email}
            onChange={handleChange}
          />
          {errors.email && <small className="text-danger ms-1">{errors.email}</small>}

          <input
            className="form-control my-2"
            name="password"
            type="password"
            placeholder="Password"
            value={form.password}
            onChange={handleChange}
          />
          {errors.password && <small className="text-danger ms-1">{errors.password}</small>}

          <select name="role" className="form-control my-2" value={form.role} onChange={handleChange}>
            <option value="JobSeeker">JobSeeker</option>
            <option value="Employer">Employer</option>
          </select>

          <div className="text-center">
            <button className="btn text-white w-50" style={{ backgroundColor: '#02122e' }}>
              Register
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
