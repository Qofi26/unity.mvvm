#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QModules.MVVM
{
    [DisallowMultipleComponent]
    public abstract class AbstractView : MonoBehaviour, IView
    {
        [Header("Base")] [SerializeField] private RectTransform _rectTransform = null!;

        [SerializeField] private CanvasGroup _canvasGroup = null!;

        [Space(20)] [SerializeField] private bool _activateWithParent = true;

        public bool IsActive { get; private set; }
        public float TickInterval { get; set; }

        public bool IsInitialized => ViewModel.IsInitialized;
        public bool ActivateWithParent => _activateWithParent;

        public RectTransform RectTransform => _rectTransform;
        public CanvasGroup CanvasGroup => _canvasGroup;

        private float _deltaTime;
        private IViewModel? _viewModel;

        protected virtual IViewFactory ViewFactory => _viewFactory;

        private IViewFactory _viewFactory = null!;

        protected IViewModel ViewModel
        {
            get
            {
                _viewModel ??= CreateViewModel();
                return _viewModel;
            }
        }

        internal readonly ICollection<IDisposable> DisposableDeinitialize = new HashSet<IDisposable>();
        internal readonly ICollection<IDisposable> DisposableDeactivate = new HashSet<IDisposable>();

        private readonly List<IView> _views = new();
        private readonly List<IView> _staticViews = new();
        private readonly List<IView> _dynamicViews = new();

        protected virtual IViewModel CreateViewModel()
        {
            return new EmptyViewModel();
        }

        public void Initialize(IViewFactory? viewFactory = null)
        {
            if (IsInitialized)
            {
                return;
            }

            _viewFactory = viewFactory ?? new DefaultViewFactory();

            ViewModel.Initialize();

            UpdateStaticViews(transform, _staticViews);
            _views.AddRange(_staticViews);

            foreach (var view in _staticViews)
            {
                view.Initialize(ViewFactory);
            }

            OnInitialize();
        }

        public void Deinitialize()
        {
            if (!IsInitialized)
            {
                return;
            }

            Deactivate();

            foreach (var view in _views)
            {
                view.Deinitialize();
            }

            OnDeinitialize();

            ViewModel.Deinitialize();

            foreach (var disposable in DisposableDeinitialize)
            {
                disposable.Dispose();
            }

            DisposableDeinitialize.Clear();

            DestroyAllDynamicViews();

            _views.Clear();
            _staticViews.Clear();
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

            IsActive = true;

            SetVisible(true);

            foreach (var view in _staticViews)
            {
                if (view.ActivateWithParent)
                {
                    view.Activate();
                }
            }

            OnActivate();
            UpdateView();
        }

        public void Deactivate()
        {
            CheckInitialized();

            if (!IsActive)
            {
                return;
            }

            IsActive = false;

            SetVisible(false);

            foreach (var view in _views)
            {
                view.Deactivate();
            }

            OnDeactivate();

            ViewModel.Deactivate();

            foreach (var disposable in DisposableDeactivate)
            {
                disposable.Dispose();
            }

            DisposableDeactivate.Clear();

            _deltaTime = 0;

            DestroyAllDynamicViews();
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

        public bool TryGetView<TView>(out TView view) where TView : IView
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

        public void UpdateView()
        {
            OnViewUpdated();
        }

        public void UpdateViewModel()
        {
            ViewModel.UpdateViewModel();
        }

        public virtual void SetInteractable(bool interactable)
        {
            if (CanvasGroup != null)
            {
                CanvasGroup.interactable = interactable;
            }
        }

        protected TView CreateView<TView>(TView prefab, Transform parent, bool activate = true) where TView : IView
        {
            var view = CreateViewInternal(prefab, parent);
            view.Initialize(ViewFactory);
            view.SetActive(activate);

            return view;
        }

        protected TView CreateView<TView, TViewArgs>(
            TView prefab,
            TViewArgs args,
            Transform parent,
            bool activate = true)
            where TView : IView<TViewArgs>
        {
            var view = CreateViewInternal(prefab, parent);
            view.Initialize(ViewFactory);

            if (activate)
            {
                view.Activate(args);
            }
            else
            {
                view.Deactivate();
            }

            return view;
        }

        protected void DestroyViews<TView>(ICollection<TView> views) where TView : IView
        {
            foreach (var view in views)
            {
                if (view == null)
                {
                    continue;
                }

                DestroyView(view);
            }

            views.Clear();
        }

        protected bool DestroyView(IView view)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (view == null)
            {
                return false;
            }

            if (!_dynamicViews.Remove(view))
            {
                Debug.LogError(
                    $"[{GetType().Name}] [{nameof(DestroyView)}] View not found in dynamic view collection. ViewName={view.name}");
                return false;
            }

            _views.Remove(view);

            view.Deinitialize();
            DestroyViewInternal(view);
            return true;
        }

        protected void DestroyAllDynamicViews()
        {
            while (_dynamicViews.Count > 0)
            {
                var view = _dynamicViews[0];
                DestroyView(view);
            }
        }

        protected virtual void OnValidate()
        {
            if (!_rectTransform)
            {
                _rectTransform = (RectTransform) transform;
            }

            if (!_canvasGroup)
            {
                TryGetComponent(out _canvasGroup);
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

        private TView CreateViewInternal<TView>(TView prefab, Transform parent)
            where TView : IView
        {
            var view = ViewFactory.InstantiateView(prefab, parent);
            AddDynamicNestedViewInternal(view);
            return view;
        }

        private void DestroyViewInternal(IView view)
        {
            ViewFactory.DestroyView(view);
        }

        protected virtual void OnViewUpdated() { }

        protected virtual void OnInitialize() { }

        protected virtual void OnDeinitialize() { }

        protected virtual void OnActivate() { }

        protected virtual void OnUpdate(float deltaTime) { }

        protected virtual void OnDeactivate() { }

        protected virtual float GetTickInterval()
        {
            return TickInterval;
        }

        private void SetVisible(bool isActive)
        {
            if (gameObject.activeSelf != isActive)
            {
                gameObject.SetActive(isActive);
            }
        }

        private void CheckInitialized()
        {
            if (IsInitialized)
            {
                return;
            }

            Debug.LogError($"[{GetType().Name}] View not initialized! ViewName={name}");
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

        private void AddDynamicNestedViewInternal(IView view)
        {
            _views.Add(view);
            _dynamicViews.Add(view);
        }
    }

    public abstract class AbstractView<TViewModel> : AbstractView where TViewModel : IViewModel, new()
    {
        protected new TViewModel ViewModel => (TViewModel) base.ViewModel;

        protected override IViewModel CreateViewModel()
        {
            return new TViewModel();
        }
    }

    public abstract class AbstractView<TViewModel, TArgs> : AbstractView<TViewModel>, IView<TArgs>
        where TViewModel : IViewModel<TArgs>, new()
    {
        public TArgs Args => ViewModel.Args;

        public void Activate(TArgs args)
        {
            if (IsActive)
            {
                SetArgs(args);
                return;
            }

            ViewModel.Activate(args);
            Activate();
        }

        public void SetArgs(TArgs args)
        {
            ViewModel.SetArgs(args);
            OnArgsChanged();
            UpdateView();
        }

        protected virtual void OnArgsChanged() { }
    }

    public abstract class AbstractViewWithArgs<TArgs> : AbstractView<EmptyViewModel<TArgs>, TArgs> { }
}
