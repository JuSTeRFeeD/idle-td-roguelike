using Project.Runtime.ECS.Components;
using Project.Runtime.Features.CameraControl;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Project.Runtime.ECS.Systems
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class CameraMoveSystem : ISystem
    {
        [Inject] private CameraController _cameraController;

        private Filter _isBuildingFilter;
        private Filter _lvlUpFilter;

        private Vector3 _dragOrigin;
        private Vector3 _currentVelocity;
        private Vector3 _targetVelocity;
        private bool _isDrag;
        private const float DragSpeed = 5f;
        private const float InertiaDamping = 4f; // Коэффициент демпфирования инерции
        private const float SmoothTime = 0.1f; // Время для сглаживания движения при зажатой мыши

        public World World { get; set; }

        public void OnAwake()
        {
            _isBuildingFilter = World.Filter
                .With<PlacingBuildingCard>()
                .Build();
            _lvlUpFilter = World.Filter
                .With<LevelUp>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_isBuildingFilter.IsNotEmpty() || // Нет необходимости двигать камеру во время постройки
                _lvlUpFilter.IsNotEmpty()) // Сброс движения во время лвл апа
            {
                _isDrag = false;
                _currentVelocity = Vector3.zero;
                _targetVelocity = Vector3.zero;
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isDrag = false;
            }

            if (!_isDrag)
            {
                if (Input.GetMouseButtonDown(0) && EventSystem.current && !EventSystem.current.IsPointerOverGameObject())
                {
                    _dragOrigin = Input.mousePosition;
                    _isDrag = true;
                }
            }

            var dt = Time.unscaledDeltaTime;
            
            var difference = Vector3.zero;
            if (_isDrag && Input.GetMouseButton(0))
            {
                var currentMousePos = Input.mousePosition;
                difference = _dragOrigin - currentMousePos;
                _dragOrigin = currentMousePos;

                var mainCameraTransform = _cameraController.MainCamera.transform;
                var forward = mainCameraTransform.forward * difference.y;
                var right = mainCameraTransform.right * difference.x;
                var move = forward + right;
                move.y = 0;

                var additionalSpeedByDist = difference.magnitude * .2f;
                _targetVelocity = move.normalized * DragSpeed * additionalSpeedByDist;

                // Плавная интерполяция текущей скорости к целевой во время зажатия мыши
                _currentVelocity = Vector3.Lerp(_currentVelocity, _targetVelocity, SmoothTime / dt);
            }

            // Плавное замедление после отпускания мыши
            if (!_isDrag && _currentVelocity != Vector3.zero)
            {
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, InertiaDamping * dt);
            }

            // Обновление позиции камеры
            var newPos = _cameraController.OriginTargetPosition + _currentVelocity * dt;
            _cameraController.SetPosition(ClampToWorld(newPos));
        }

        private readonly RectInt _bounds = new(-5, -5, 65, 65);
        private Vector3 ClampToWorld(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, _bounds.xMin, _bounds.xMax);
            position.z = Mathf.Clamp(position.z, _bounds.yMin, _bounds.yMax);
            return position;
        }

        public void Dispose()
        {
        }
    }
}
