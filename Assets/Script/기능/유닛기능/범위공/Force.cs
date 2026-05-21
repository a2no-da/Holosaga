using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Force : MonoBehaviour
{
    float duration = 0.3f;

    protected struct ForceApplication
    {
        public Rigidbody2D targetRb;
        public Vector2 force;
        public float startTime;
        public float duration;
        public float endTime;
        public Vector2 initialForcePerSecond;
    }

    protected List<ForceApplication> forceApplications = new List<ForceApplication>();

    protected void ApplyForce(Rigidbody2D targetRb, Vector2 force)
    {
        Enemy enemyScript = targetRb.GetComponent<Enemy>();
        if (enemyScript != null && enemyScript.Laplus)
        {
            return;
        }

        float startTime = Time.time;
        float endTime = startTime + duration;

        Vector2 initialForcePerSecond = force / duration;

        bool forceApplied = false;
        for (int i = 0; i < forceApplications.Count; i++)
        {
            if (forceApplications[i].targetRb == targetRb)
            {
                var tempForceApplication = forceApplications[i];

                tempForceApplication.force = force;
                tempForceApplication.initialForcePerSecond = initialForcePerSecond;
                tempForceApplication.startTime = startTime;
                tempForceApplication.endTime = endTime;

                forceApplications[i] = tempForceApplication;
                forceApplied = true;
                break;
            }
        }

        if (!forceApplied)
        {
            forceApplications.Add(new ForceApplication
            {
                targetRb = targetRb,
                force = force,
                startTime = startTime,
                duration = duration,
                endTime = endTime,
                initialForcePerSecond = initialForcePerSecond
            });
        }
    }

    protected virtual void Update()
    {
        float currentTime = Time.time;

        for (int i = forceApplications.Count - 1; i >= 0; i--)
        {
            var application = forceApplications[i];
            if (currentTime < application.endTime && application.targetRb != null)
            {

                Fo(currentTime);
            }
            else if (currentTime >= application.endTime && application.targetRb != null)
            {
                application.targetRb.velocity = Vector2.zero;

                Enemy enemyScript = application.targetRb.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.isPushedOrPulled = false;
                }
                forceApplications.RemoveAt(i);
            }
        }
    }

    public void Fo(float currentTime)
    {
        for (int i = forceApplications.Count - 1; i >= 0; i--)
        {
            var application = forceApplications[i];
            if (currentTime < application.endTime && application.targetRb != null)
            {
                Enemy enemyScript = application.targetRb.GetComponent<Enemy>();
                if (enemyScript != null && enemyScript.Laplus)
                {
                    continue;
                }

                application.targetRb.AddForce(application.force, ForceMode2D.Impulse);

                if (application.force.x > 0)
                {
                    application.force.x -= 1f;
                    application.force.x = Mathf.Max(application.force.x, 0f);
                }

                if (application.force.x < 0)
                {
                    application.force.x += 1f;
                    application.force.x = Mathf.Min(application.force.x, 0f);
                }

                forceApplications[i] = application;
            }
        }
    }

    public void PublicApplyForce(Rigidbody2D targetRb, Vector2 force)
    {
        ApplyForce(targetRb, force);
    }
}

