using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;

public class NewCinemachineCameraController : CinemachineCameraController
{
    /// <summary>
    ///     Starts following the LevelManager's main player
    /// </summary>
    public override void StartFollowing()
    {
        if (!FollowsAPlayer) return;
        FollowsPlayer = true;
        if (TargetCharacter.CameraTarget != null)
        {
            _virtualCamera.Follow = TargetCharacter.CameraTarget.transform;
            _virtualCamera.enabled = true;
        }
    }
}
