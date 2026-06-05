using UnityEngine;

namespace Tracking
{
    public static class MotionComparer
    {
        // Compara dos grabaciones y devuelve un score 0..100 (100 = idéntico)
        // Métodos disponibles:
        // - simple resample + mean distance (Compare)
        // - DTW (CompareDtw) para tolerancia temporal

        public static float Compare(MotionRecording a, MotionRecording b, int targetSamples = 100, float maxErrorMeters = 0.8f)
        {
            // fallback to original simple resample method
            if (a == null || b == null) return 0f;
            if (a.Frames == null || b.Frames == null) return 0f;
            if (a.Frames.Length == 0 || b.Frames.Length == 0) return 0f;

            Vector3[] aLeft = GetResampledPositions(a, true, targetSamples);
            Vector3[] aRight = GetResampledPositions(a, false, targetSamples);
            Vector3[] bLeft = GetResampledPositions(b, true, targetSamples);
            Vector3[] bRight = GetResampledPositions(b, false, targetSamples);

            float total = 0f;
            for (int i = 0; i < targetSamples; i++)
            {
                float dl = Vector3.Distance(aLeft[i], bLeft[i]);
                float dr = Vector3.Distance(aRight[i], bRight[i]);
                total += (dl + dr) * 0.5f; // average hands
            }
            float meanError = total / targetSamples;

            return MapErrorToScore(meanError, maxErrorMeters);
        }

        public static float CompareDtw(MotionRecording a, MotionRecording b, int maxSamples = 120, float maxErrorMeters = 0.8f)
        {
            if (a == null || b == null) return 0f;
            if (a.Frames == null || b.Frames == null) return 0f;
            if (a.Frames.Length == 0 || b.Frames.Length == 0) return 0f;

            // Resample both to at most maxSamples to keep DTW cost reasonable
            int samplesA = Mathf.Min(a.Frames.Length, maxSamples);
            int samplesB = Mathf.Min(b.Frames.Length, maxSamples);

            Vector3[] aLeft = GetResampledPositions(a, true, samplesA);
            Vector3[] aRight = GetResampledPositions(a, false, samplesA);
            Vector3[] bLeft = GetResampledPositions(b, true, samplesB);
            Vector3[] bRight = GetResampledPositions(b, false, samplesB);

            // Build cost matrix and run DTW
            float[,] cost = new float[samplesA + 1, samplesB + 1];
            const float inf = 1e9f;
            for (int i = 0; i <= samplesA; i++) for (int j = 0; j <= samplesB; j++) cost[i, j] = inf;
            cost[0, 0] = 0f;

            for (int i = 1; i <= samplesA; i++)
            {
                for (int j = 1; j <= samplesB; j++)
                {
                    float dL = Vector3.Distance(aLeft[i - 1], bLeft[j - 1]);
                    float dR = Vector3.Distance(aRight[i - 1], bRight[j - 1]);
                    float d = 0.5f * (dL + dR);
                    float minPrev = Mathf.Min(cost[i - 1, j], Mathf.Min(cost[i, j - 1], cost[i - 1, j - 1]));
                    cost[i, j] = d + minPrev;
                }
            }

            float totalCost = cost[samplesA, samplesB];
            // Normalize by path length estimate (average of lengths)
            float pathLen = (samplesA + samplesB) * 0.5f;
            float meanError = totalCost / Mathf.Max(1f, pathLen);

            return MapErrorToScore(meanError, maxErrorMeters);
        }

        static float MapErrorToScore(float meanError, float maxErrorMeters)
        {
            float t = Mathf.Clamp01(1f - (meanError / maxErrorMeters));
            float score = Mathf.Pow(t, 1.2f) * 100f;
            return score;
        }

        static Vector3[] GetResampledPositions(MotionRecording rec, bool left, int samples)
        {
            Vector3[] outPos = new Vector3[samples];
            int n = rec.Frames.Length;
            if (n == 0)
            {
                for (int i = 0; i < samples; i++) outPos[i] = Vector3.zero;
                return outPos;
            }

            float totalDuration = rec.Frames[n - 1].time;
            if (totalDuration <= 0f) totalDuration = 1f; // avoid div0

            for (int i = 0; i < samples; i++)
            {
                float tNorm = samples == 1 ? 0f : (float)i / (samples - 1);
                float targetTime = tNorm * totalDuration;
                outPos[i] = InterpolatePosition(rec.Frames, targetTime, left);
            }
            return outPos;
        }

        static Vector3 InterpolatePosition(MotionFrame[] frames, float time, bool left)
        {
            if (frames == null || frames.Length == 0) return Vector3.zero;
            if (time <= frames[0].time) return left ? frames[0].leftPos : frames[0].rightPos;
            if (time >= frames[frames.Length - 1].time) return left ? frames[frames.Length - 1].leftPos : frames[frames.Length - 1].rightPos;

            // find interval
            int i = 0;
            while (i < frames.Length - 1 && frames[i + 1].time < time) i++;
            MotionFrame f0 = frames[i];
            MotionFrame f1 = frames[i + 1];
            float dt = f1.time - f0.time;
            float u = dt > 0f ? (time - f0.time) / dt : 0f;
            Vector3 p0 = left ? f0.leftPos : f0.rightPos;
            Vector3 p1 = left ? f1.leftPos : f1.rightPos;
            return Vector3.Lerp(p0, p1, u);
        }
    }
}


