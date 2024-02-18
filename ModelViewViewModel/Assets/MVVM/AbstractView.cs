#nullable enable

#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Erem.MVVM
{
    [DisallowMultipleComponent]
    public abstract class AbstractView<T> : MonoBehaviour, IView
        where T : IViewModel, new()
    {
        [SerializeField]
        private bool _activateWithParent = true;

        [SerializeField]
        private CanvasGroup _canvasGroup = null!;

        protected float TickInterval { get; set; }

        private float _deltaTime;

        private T? _viewModel;
        public bool IsActive { get; private set; }

        public bool ActivateWithParent => _activateWithParent;
        public RectTransform RectTransform => (RectTransform) transform;

        public CanvasGroup CanvasGroup => _canvasGroup;

        public IView? Owner { get; private set; }
        public bool IsInitialized => ViewModel.IsInitialized;

        protected T ViewModel
        {
            get
            {
                _viewModel ??= new T();
                return _viewModel;
            }
        }

        private readonly List<IView> _views = new();
        private readonly List<IView> _staticViews = new();
        private readonly List<IView> _dynamicViews = new();

        protected virtual void Update()
        {
            if (!IsActive || !enabled || !gameObject.activeInHierarchy)
            {
                return;
            }

            var deltaTime = Time.deltaTime;

            _deltaTime += deltaTime;

            if (_deltaTime >= TickInterval)
            {
                OnUpdate(_deltaTime);
                _deltaTime = 0;
            }
        }

        protected virtual void OnValidate()
        {
            if (_canvasGroup == null)
            {
                TryGetComponent(out _canvasGroup);
            }
        }

        public void Initialize(IView owner)
        {
            if (IsInitialized)
            {
                return;
            }

            Owner = owner;

            _viewModel ??= new T();
            ViewModel.Initialize();

            _views.Clear();
            _staticViews.Clear();

            UpdateStaticViews(transform, _staticViews);
            _views.AddRange(_staticViews);

            foreach (var view in _staticViews)
            {
                view.Initialize(this);
            }

            OnInitialize();
        }

        public void Shutdown()
        {
            if (!IsInitialized)
            {
                return;
            }

            foreach (var view in _views)
            {
                view.Shutdown();
            }

            Deactivate();

            ViewModel.Shutdown();
            OnShutdown();

            Owner = null;
        }

        public void Activate()
        {
            CheckInitialized();

            if (IsActive)
            {
                return;
            }

            _deltaTime = 0;

            ViewModel.Activate();
            SetVisible(true);
            IsActive = true;

            foreach (var view in _staticViews)
            {
                if (view.ActivateWithParent)
                {
                    view.Activate();
                }
            }

            OnActivate();
        }

        public void Deactivate()
        {
            CheckInitialized();

            if (!IsActive)
            {
                return;
            }

            ViewModel.Deactivate();
            SetVisible(false);
            IsActive = false;

            foreach (var view in _views)
            {
                view.Deactivate();
            }

            OnDeactivate();

            // TODO: DestroyAllDynamicWidgets

            _deltaTime = 0;
            TickInterval = 0;

            _views.Clear();
            _staticViews.Clear();
            _dynamicViews.Clear();
        }

        public void SetActive(bool isActive)
        {
            if (isActive)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        public bool TryGetView<TView>(out TView view)
        {
            foreach (var element in _staticViews)
            {
                if (element is TView tView)
                {
                    view = tView;
                    return true;
                }
            }

            view = default!;
            return false;
        }

        protected virtual TView CreateView<TView>(TView prefab, Transform parent, bool activate = true)
            where TView : IView
        {
            var view = CreateViewInternal(prefab, parent);
            view.Initialize(this);

            if (activate)
            {
                view.Activate();
            }
            else
            {
                view.Deactivate();
            }

            _views.Add(view);
            _dynamicViews.Add(view);
            return view;
        }

        protected virtual TView CreateView<TView, TViewArgs>(TView prefab,
            TViewArgs args,
            Transform parent,
            bool activate = true)
            where TView : IView<TViewArgs>
        {
            var view = CreateViewInternal(prefab, parent);
            view.Initialize(this);

            if (activate)
            {
                view.Activate(args);
            }
            else
            {
                view.Deactivate();
            }

            _views.Add(view);
            _dynamicViews.Add(view);
            return view;
        }

        public void DestroyView(IView view)
        {
            if (!_dynamicViews.Remove(view))
            {
                return;
            }

            _views.Remove(view);

            view.Shutdown();
            DestroyViewInternal(view);
        }

        protected virtual TView CreateViewInternal<TView>(TView prefab, Transform parent)
            where TView : IView
        {
            if (prefab is not Object obj)
            {
                return default!;
            }

            var instance = Instantiate(obj, parent);

            if (instance is TView view)
            {
                return view;
            }

            return default!;
        }

        protected virtual void DestroyViewInternal(IView view)
        {
            if (view is not Object obj)
            {
                return;
            }

            Destroy(obj);
        }

        public void DestroyAllDynamicViews()
        {
            while (_dynamicViews.Count > 0)
            {
                var view = _dynamicViews[0];
                DestroyView(view);
            }
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnShutdown()
        {
        }

        protected virtual void OnActivate()
        {
        }

        protected virtual void OnUpdate(float deltaTime)
        {
        }

        protected virtual void OnDeactivate()
        {
        }

        private void SetVisible(bool isActive)
        {
            if (gameObject.activeSelf == isActive)
            {
                return;
            }

            gameObject.SetActive(isActive);
        }

        private void CheckInitialized()
        {
            if (IsInitialized)
            {
                return;
            }

            Debug.LogError($"View {name} not initialized!");
        }

        private void UpdateStaticViews(Transform target, List<IView> views)
        {
            foreach (Transform child in target)
            {
                if (child.TryGetComponent(out IView view))
                {
                    views.Add(view);
                }
                else
                {
                    UpdateStaticViews(child, views);
                }
            }
        }
    }

    [DisallowMultipleComponent]
    public abstract class AbstractView<TViewModel, TArgs> : AbstractView<TViewModel>
        where TViewModel : IViewModel<TArgs>, new()
    {
        public TArgs Args => ViewModel.Args;

        public void Activate(TArgs args)
        {
            ViewModel.Activate(args);
            OnActivate();
        }

        public void SetArgs(TArgs args)
        {
            ViewModel.SetArgs(args);
            OnArgsChanged();
        }

        protected virtual void OnArgsChanged()
        {
        }
    }
}