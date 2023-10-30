﻿namespace Zinnia.Tracking.Modification
{
    using UnityEngine;
    using Zinnia.Data.Type;
    using Zinnia.Extension;
    using Zinnia.Process;

    /// <summary>
    /// Scales a given target based on the distance between two points.
    /// </summary>
    public class PinchScaler : MonoBehaviour, IProcessable
    {
        [Header("Scale Settings")]
        [Tooltip("The target to scale.")]
        [SerializeField]
        private GameObject target;
        /// <summary>
        /// The target to scale.
        /// </summary>
        public GameObject Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
        [Tooltip("The point to determine distance from.")]
        [SerializeField]
        private GameObject primaryPoint;
        /// <summary>
        /// The point to determine distance from.
        /// </summary>
        public GameObject PrimaryPoint
        {
            get
            {
                return primaryPoint;
            }
            set
            {
                primaryPoint = value;
                if (this.IsMemberChangeAllowed())
                {
                    OnAfterPrimaryPointChange();
                }
            }
        }
        [Tooltip("The point to determine distance to.")]
        [SerializeField]
        private GameObject secondaryPoint;
        /// <summary>
        /// The point to determine distance to.
        /// </summary>
        public GameObject SecondaryPoint
        {
            get
            {
                return secondaryPoint;
            }
            set
            {
                secondaryPoint = value;
                if (this.IsMemberChangeAllowed())
                {
                    OnAfterSecondaryPointChange();
                }
            }
        }
        [Tooltip("Determines whether to use local or global scale.")]
        [SerializeField]
        private bool useLocalScale = true;
        /// <summary>
        /// Determines whether to use local or global scale.
        /// </summary>
        public bool UseLocalScale
        {
            get
            {
                return useLocalScale;
            }
            set
            {
                useLocalScale = value;
            }
        }
        [Tooltip("Determines whether to calculate the multiplier using Mathf.Pow(float, float).")]
        [SerializeField]
        private bool calculateByPower;
        /// <summary>
        /// Determines whether to calculate the multiplier using <see cref="Mathf.Pow(float, float)"/>.
        /// </summary>
        public bool CalculateByPower
        {
            get
            {
                return calculateByPower;
            }
            set
            {
                calculateByPower = value;
            }
        }
        [Tooltip("A scale factor multiplier.")]
        [SerializeField]
        private float multiplier = 1f;
        /// <summary>
        /// A scale factor multiplier.
        /// </summary>
        public float Multiplier
        {
            get
            {
                return multiplier;
            }
            set
            {
                multiplier = value;
            }
        }

        [Header("Restriction Settings")]
        [Tooltip("Determines which axes to apply the modification on>.")]
        [SerializeField]
        private Vector3State applyScaleOnAxis = Vector3State.True;
        /// <summary>
        /// Determines which axes to apply the modification on>.
        /// </summary>
        public Vector3State ApplyScaleOnAxis
        {
            get
            {
                return applyScaleOnAxis;
            }
            set
            {
                applyScaleOnAxis = value;
            }
        }
        [Tooltip("The minimum allowed scale.")]
        [SerializeField]
        private Vector3 minimumScaleLimit = Vector3.one * float.NegativeInfinity;
        /// <summary>
        /// The minimum allowed scale.
        /// </summary>
        public Vector3 MinimumScaleLimit
        {
            get
            {
                return minimumScaleLimit;
            }
            set
            {
                minimumScaleLimit = value;
            }
        }
        [Tooltip("The maximum allowed scale.")]
        [SerializeField]
        private Vector3 maximumScaleLimit = Vector3.one * float.PositiveInfinity;
        /// <summary>
        /// The maximum allowed scale.
        /// </summary>
        public Vector3 MaximumScaleLimit
        {
            get
            {
                return maximumScaleLimit;
            }
            set
            {
                maximumScaleLimit = value;
            }
        }

        /// <summary>
        /// The previous distance between <see cref="PrimaryPoint"/> and <see cref="SecondaryPoint"/>.
        /// </summary>
        protected float? previousDistance;
        /// <summary>
        /// The original scale of <see cref="Target"/>.
        /// </summary>
        protected Vector3 originalScale;

        /// <summary>
        /// Clears <see cref="Target"/>.
        /// </summary>
        public virtual void ClearTarget()
        {
            if (!this.IsValidState())
            {
                return;
            }

            Target = default;
        }

        /// <summary>
        /// Clears <see cref="PrimaryPoint"/>.
        /// </summary>
        public virtual void ClearPrimaryPoint()
        {
            if (!this.IsValidState())
            {
                return;
            }

            PrimaryPoint = default;
        }

        /// <summary>
        /// Clears <see cref="SecondaryPoint"/>.
        /// </summary>
        public virtual void ClearSecondaryPoint()
        {
            if (!this.IsValidState())
            {
                return;
            }

            SecondaryPoint = default;
        }

        /// <summary>
        /// Processes the current scale factor onto the target.
        /// </summary>
        public virtual void Process()
        {
            if (!this.IsValidState() || Target == null || PrimaryPoint == null || SecondaryPoint == null)
            {
                return;
            }

            Scale();
        }

        /// <summary>
        /// Saves the existing target scale.
        /// </summary>
        public virtual void SaveCurrentScale()
        {
            originalScale = GetTargetScale();
        }

        /// <summary>
        /// Restores the saved target scale.
        /// </summary>
        public virtual void RestoreSavedScale()
        {
            if (UseLocalScale)
            {
                Target.transform.localScale = originalScale;
            }
            else
            {
                Target.transform.SetGlobalScale(originalScale);
            }
        }

        protected virtual void OnDisable()
        {
            previousDistance = null;
        }

        /// <summary>
        /// Attempts to scale the target.
        /// </summary>
        protected virtual void Scale()
        {
            previousDistance = previousDistance == null ? GetDistance() : previousDistance;

            if (CalculateByPower)
            {
                float previousDistanceValue = (float)previousDistance;
                if (!previousDistanceValue.ApproxEquals(0))
                {
                    ScaleByPower();
                }
            }
            else
            {
                ScaleByMultiplier();
            }

            previousDistance = GetDistance();
        }

        /// <summary>
        /// Applies the scale limits to the given scale value.
        /// </summary>
        /// <param name="scale">The scale value to apply limits to.</param>
        /// <returns>The scale value with limits applied.</returns>
        protected virtual Vector3 ApplyLimits(Vector3 scale)
        {
            return new Vector3(
                ApplyScaleOnAxis.xState ? Mathf.Clamp(scale.x, minimumScaleLimit.x, maximumScaleLimit.x) : GetTargetScale().x,
                ApplyScaleOnAxis.yState ? Mathf.Clamp(scale.y, minimumScaleLimit.y, maximumScaleLimit.y) : GetTargetScale().y,
                ApplyScaleOnAxis.zState ? Mathf.Clamp(scale.z, minimumScaleLimit.z, maximumScaleLimit.z) : GetTargetScale().z
                );
        }

        /// <summary>
        /// Scales the object by the distance delta multiplied against the multiplier.
        /// </summary>
        protected virtual void ScaleByMultiplier()
        {
            float distanceDelta = GetDistance() - (float)previousDistance;
            Vector3 newScale = Vector3.one * distanceDelta * Multiplier;
            if (UseLocalScale)
            {
                Target.transform.localScale = ApplyLimits(Target.transform.localScale + newScale);
            }
            else
            {
                Target.transform.SetGlobalScale(ApplyLimits(Target.transform.lossyScale + newScale));
            }
        }

        /// <summary>
        /// Scales the object using a power of the multiplier.
        /// </summary>
        protected virtual void ScaleByPower()
        {
            float scaleRatio = GetDistance() / (float)previousDistance;
            Vector3 scaleVector = Vector3.one * Mathf.Pow(scaleRatio, Multiplier);

            if (UseLocalScale)
            {
                Target.transform.localScale = ApplyLimits(Vector3.Scale(Target.transform.localScale, scaleVector));
            }
            else
            {
                Target.transform.SetGlobalScale(ApplyLimits(Vector3.Scale(Target.transform.lossyScale, scaleVector)));
            }
        }

        /// <summary>
        /// Gets the distance between the primary point and secondary point;
        /// </summary>
        /// <returns>The distance between the points.</returns>
        protected virtual float GetDistance()
        {
            return Vector3.Distance(PrimaryPoint.transform.position, SecondaryPoint.transform.position);
        }

        /// <summary>
        /// Gets the scale of the target in either local or global scale.
        /// </summary>
        /// <returns>The scale of the target.</returns>
        protected virtual Vector3 GetTargetScale()
        {
            return UseLocalScale ? Target.transform.localScale : Target.transform.lossyScale;
        }

        /// <summary>
        /// Called after <see cref="PrimaryPoint"/> has been changed.
        /// </summary>
        protected virtual void OnAfterPrimaryPointChange()
        {
            previousDistance = null;
        }

        /// <summary>
        /// Called after <see cref="SecondaryPoint"/> has been changed.
        /// </summary>
        protected virtual void OnAfterSecondaryPointChange()
        {
            previousDistance = null;
        }
    }
}