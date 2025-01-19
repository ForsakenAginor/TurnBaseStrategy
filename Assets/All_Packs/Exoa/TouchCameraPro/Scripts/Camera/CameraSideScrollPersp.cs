using Exoa.Designer;
using Exoa.Events;
using Exoa.Touch;
using System.Collections.Generic;
using UnityEngine;

namespace Exoa.Cameras
{
    public class CameraSideScrollPersp : CameraPerspBase, ITouchPerspCamera
    {
        [Header("PLANE")]
        public Exoa.Common.Plane plane;

        override protected void CreateConverter()
        {
            HeightScreenDepth = new ScreenDepth(ScreenDepth.ConversionType.PlaneIntercept);
            HeightScreenDepth.Object = plane;
        }

        /// <summary>
        /// Init some camera parameters
        /// </summary>
        override protected void Init()
        {
            Log("Init position:" + transform.position);

            base.Init();

            initialRotation = new Vector2(0, plane.transform.rotation.eulerAngles.y);

            currentPitch = initialRotation.x;
            currentYaw = initialRotation.y;
            GetInitialRotation();
            initDistance = Vector3.Distance(plane.GetClosest(transform.position), transform.position);

            finalDistance = initDistance;
            finalOffset = CalculateOffset(transform.position, transform.rotation, initDistance, groundHeight);
            finalRotation = GetRotationFromPitchYaw();
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

            allowPitchRotation = false;
        }




        protected override void Update()
        {
            currentPitch = plane.transform.rotation.eulerAngles.x;
            currentYaw = plane.transform.rotation.eulerAngles.y;
            RotateFromVector(CameraInputs.GetAnyPixelScaledDelta());
            finalRotation = GetRotationFromPitchYaw();

            base.Update();
        }


        #region RESET


        /// <summary>
        /// Set the initial values for the reset function
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="rotation"></param>
        /// <param name="distanceOrSize"></param>
        override public void SetResetValues(Vector3 offset, Quaternion rotation, float distance)
        {
            initOffset = offset;
            initDistance = distance;
            initialRotation = rotation.eulerAngles;
            GetInitialRotation();
            //Log("SetResetValues initOffset:" + initOffset);
        }


        /// <summary>
        /// Reset the camera to initial values
        /// </summary>
        override public void ResetCamera()
        {
            StopFollow();
            FocusCamera(initOffset, initDistance, initRotation.eulerAngles);
        }
        #endregion

        /// <summary>
        /// Gives the initial rotation of the camera to be able to reset it later
        /// </summary>
        /// <returns></returns>
        override protected Quaternion GetInitialRotation()
        {
            initRotation = Quaternion.Euler(initialRotation.x, initialRotation.y, 0);
            return initRotation;
        }

        #region EVENTS
        override protected Vector2 GetRotationToPitchYaw(Quaternion rot)
        {
            rot = plane.transform.rotation;
            return new Vector2(NormalizeAngle(rot.eulerAngles.x), rot.eulerAngles.y);
        }

        override protected Quaternion GetRotationFromPitchYaw()
        {
            currentPitch = plane.transform.rotation.eulerAngles.x;
            currentYaw = plane.transform.rotation.eulerAngles.y;
            currentPitch = NormalizeAngle(currentPitch);
            currentYaw = NormalizeAngle(currentYaw);
            return Quaternion.Euler(currentPitch, currentYaw, 0);

        }


        /// <summary>
        /// Clamp any given point inside the boundaries collider on XY plan. Z will be unchanged
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        override protected Vector3 ClampInCameraBoundaries(Vector3 targetPosition, out bool isInBoundaries)
        {
            bool anyFingerDown = CameraInputs.GetFingerCount() > 0;
            isInBoundaries = true;
            if (camBounds != null)
                return camBounds.ClampInBoundsXY(targetPosition, out isInBoundaries, groundHeight, anyFingerDown || isApplyingMoveInertia);
            return targetPosition;
        }



        /// <summary>
        /// Calculate the offset position on the ground, given the camera's position and rotation
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        override protected Vector3 CalculateOffset(Vector3 pos, Quaternion rot)
        {
            Vector3 camOffset = plane.GetClosest(pos);
            return camOffset;
        }

        /// <summary>
        /// Calculates the camera transform's position giving the offset, rotation and distance
        /// </summary>
        /// <param name="center"></param>
        /// <param name="rot"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        override protected Vector3 CalculatePosition(Vector3 center, Quaternion rot, float distance)
        {
            return center - plane.transform.forward * distance;
        }

        /// <summary>
        /// Calculate the offset position on the ground, given the camera's position, rotation, distance from ground and ground height
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="distance"></param>
        /// <param name="groundHeight"></param>
        /// <returns></returns>
        override protected Vector3 CalculateOffset(Vector3 pos, Quaternion rot, float distance, float groundHeight)
        {
            Vector3 camOffset = plane.GetClosest(pos);
            return camOffset;
        }



        #region FOLLOW & FOCUS

        /// <summary>
        /// Handle the camera focus/follow/moveto
        /// </summary>
        override protected void HandleFocusAndFollow()
        {
            if (!isFocusingOrFollowing)
                return;

            if (focusTargetGo != null)
            {
                Bounds b = focusTargetGo.GetBoundsRecursive();
                //Log("Bounds:" + b);
                if (b.size == Vector3.zero && b.center == Vector3.zero)
                    return;

                // offseting the bounding box
                //b.center = b.center.SetZ(groundHeight);

                float aspect = cam.aspect;
                float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(fov * Mathf.Deg2Rad / 2f) * aspect) * Mathf.Rad2Deg;


                float fovMin = Mathf.Min(fov, horizontalFOV);
                float sin = (Mathf.Sin(fovMin * Mathf.Deg2Rad / 2f));
                sin = sin == 0f ? 0.000001f : sin;
                Vector3 max = b.size;
                // Get the radius of a sphere circumscribing the bounds
                float radius = max.magnitude * focusRadiusMultiplier;
                float dist = radius / sin;
                focusTargetPosition = b.center;

                focusTargetDistanceOrSize = dist * focusDistanceMultiplier;
                focusTargetDistanceOrSize = CalculateClampedDistance(focusTargetDistanceOrSize);

            }

            if (enableDistanceChange)
            {
                finalDistance = followMove.Update(ref followMoveDistanceOrSize,
                    focusTargetDistanceOrSize, OnFollowFocusCompleted);
            }

            if (enableRotationChange)
            {
                finalRotation = followMove.Update(ref followMoveRotation, focusTargetRotation);
                currentPitch = finalRotation.eulerAngles.x;
                currentYaw = finalRotation.eulerAngles.z;
            }
            finalOffset = worldPointCameraCenter = followMove.Update(ref followMoveOffset, focusTargetPosition, OnFollowFocusCompleted, true);
            finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);

        }
        #endregion






        /// <summary>
        /// Called just before the perspective switch happens
        /// </summary>
        /// <param name="orthoMode"></param>
        override protected void OnBeforeSwitchPerspective(bool orthoMode)
        {
            if (!orthoMode)
            {
                initialRotation = new Vector2(0, plane.transform.rotation.eulerAngles.y);
                currentPitch = initialRotation.x;
                currentYaw = initialRotation.y;
                finalRotation = GetRotationFromPitchYaw();
                finalPosition = CalculatePosition(finalOffset, finalRotation, finalDistance);
            }
        }

        #endregion


    }
}
