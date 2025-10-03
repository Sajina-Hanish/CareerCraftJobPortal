import bgImage from '../assets/LandingPageBackground.jpeg';

export default function LandingPage() {
  return (
    <div
      className="landing-page container-fluid d-flex align-items-center justify-content-center text-center"
      style={{
        backgroundImage: `url(${bgImage})`,
        backgroundSize: 'cover',
        backgroundPosition: 'center',
        backgroundRepeat: 'no-repeat',
        height: '92vh'
      }}
    >
      <div className="container bg-dark bg-opacity-75 text-white p-5 rounded shadow">
        <h1 className="display-4">Welcome to CareerCrafter</h1>
        <p className="lead">Find your dream job or hire the best talent.</p>
      </div>
    </div>
  );
}
