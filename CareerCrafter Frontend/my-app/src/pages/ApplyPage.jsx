import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { applyToJob } from "../services/ApplyServices";

const ApplyPage = () => {
  const { jobId } = useParams();
  const [resume, setResume] = useState(null);
  const { user } = useAuth();
  const navigate = useNavigate();

  const handleApply = async () => {
    const form = new FormData();
    form.append("jobId", jobId);
    form.append("resume", resume);

    await post("/applications", form, user.token, true);
    alert("Applied!");
    navigate("/jobseeker");
  };

  return (
    <div>
      <h2>Apply to Job #{jobId}</h2>
      <input type="file" onChange={e => setResume(e.target.files[0])} />
      <button onClick={handleApply}>Submit Application</button>
    </div>
  );
};

export default ApplyPage;
