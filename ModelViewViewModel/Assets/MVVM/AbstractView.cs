#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace Erem.MVVM
{
    /// <summary>
    /// It is not recommended to inherit from this class.<br/>
    /// Use <b>AbstractView&lt;TViewModel&gt;</b> and <b>AbstractView&lt;TViewModel, TArgs&gt;</b>
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AbstractView : MonoBehaviour, IView
    {
        [SerializeField]
        private bool _activateWithParent = true;

        [SerializeField]
        private RectTransform _rectTransform = null!;

        [SerializeField]
        private CanvasGroup _canvasGroup = null!;

        public IView? Owner { get; private set; }
        public bool IsActive { get; private set; }
        public float TickInterval { get; set; }

        public bool IsInitialized => ViewModel.IsInitialized;
        public bool ActivateWithParent => _activateWithParent;

        public RectTransform RectTransform => _rectTransform;
        public CanvasGroup CanvasGroup => _canvasGroup;

        private float _deltaTime;
        private IViewModel? _viewModel;

        protected IViewModel ViewModel
        {
            get
            {
                _viewModel ??= CreateViewModel();
                return _viewModel;
            }
        }

        private readonly List<IView> _views = new();
        private readonly List<IView> _staticViews = new();
        private readonly List<IView> _dynamicViews = new();

        protected abstract IViewModel CreateViewModel();

        public void Initialize(IView? owner)
        {
            if (IsInitialized)
            {
                return;
            }

            Owner = owner;

            ViewModel.Initialize();

            DestroyAllDynamicViews();

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

            OnShutdown();
            ViewModel.Shutdown();

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

            DestroyAllDynamicViews();

            _deltaTime = 0;

            OnDeactivate();
        }

        public bool SetActive(bool isActive)
        {
            if (isActive)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }

            return isActive;
        }

        public bool TryGetView<TView>(out TView view)
        {
            foreach (var element in _views)
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

            AddNestedViewInternal(view, activate);
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
            view.SetArgs(args);

            AddNestedViewInternal(view, activate);
            return view;
        }

        protected void DestroyView(IView view)
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

            if (Application.isPlaying)
            {
                Destroy(obj);
            }
            else
            {
                DestroyImmediate(obj);
            }
        }

        protected void DestroyAllDynamicViews()
        {
            while (_dynamicViews.Count > 0)
            {
                var view = _dynamicViews[0];
                DestroyView(view);
            }
        }

        private void Update()
        {
            if (!IsActive || !enabled || !gameObject.activeInHierarchy)
            {
                return;
            }

            _deltaTime += Time.deltaTime;

            if (_deltaTime < GetTickInterval())
            {
                return;
            }

            ViewModel.Update(_deltaTime);
            OnUpdate(_deltaTime);

            _deltaTime = 0;
        }

        protected virtual void OnValidate()
        {
            if (_rectTransform == null)
            {
                _rectTransform = (RectTransform) transform;
            }

            if (_canvasGroup == null)
            {
                TryGetComponent(out _canvasGroup);
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

        protected virtual float GetTickInterval()
        {
            return TickInterval;
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

        private void AddNestedViewInternal(IView view, bool activate)
        {
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
        }
    }

    public abstract class AbstractView<T> : AbstractView where T : IViewModel, new()
    {
        protected new T ViewModel => (T) base.ViewModel;

        protected override IViewModel CreateViewModel()
        {
            return new T();
        }
    }

    public abstract class AbstractView<TViewModel, TArgs> : AbstractView<TViewModel>, IView<TArgs>
        where TViewModel : IViewModel<TArgs>, new()
    {
        public TArgs Args => ViewModel.Args;

        public void Activate(TArgs args)
        {
            ViewModel.SetArgs(args);
            Activate();
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