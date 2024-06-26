using Cinemachine;
using UnityEngine;

namespace Foundation.Player.Views
{
    public class CameraFollowInitializer : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;

        public void SetCameraFollow(Transform followObject)
        {
            _camera.Follow = followObject;
            _camera.LookAt = followObject;
        }
    }
}