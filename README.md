# Simple Model-View-ViewModel for Unity

## Install via UPM (using Git URL)

````
https://github.com/Qofi26/unity.mvvm.git?path=/ModelViewViewModel/Assets/MVVM#v/0.1.0
````

# Basic Usage

Inherit your viewModel from AbstractViewModel

    public class ExampleViewModel : AbstractViewModel
    {
        public int Value { get; private set; }

        public void SetValue(int value)
        {
            Value = value;
        }
    }

Inherit your view from AbstractView<TViewModel>

    public class ExampleView : AbstractView<ExampleViewModel>
    {
        [SerializeField]
        private Text _text1;

        protected override void OnActivate()
        {
            UpdateView();
        }

        public void SetRandomValue()
        {
            var value = Random.Range(0, 100);
            ViewModel.SetValue(value);
            UpdateView();
        }

        private void UpdateView()
        {
            _text1.text = $"{ViewModel.Value}";
        }
    }

Then call `Initialize()` on your root View for initialization all nested views.

And call `Activate` (immediately or at any other time) method for activate root view.

Nested views activated automatically, if `ActivateWithParent` (in nested `View`) is `true`

    public class InitializerDemo : MonoBehaviour
    {
        [SerializeField]
        private ExampleView _view;

        private void Awake()
        {
            _view.Initialize(null);
            _view.Activate();
        }
    }

Also you can args for view and viewModel.

You can use any types as Args for `View` and `ViewModel`.

To do this, inherit from `AbstractViewModel<Targs>` and `AbstractView<TViewModel, TArgs>`

    public class ExampleViewModel : AbstractViewModel<object>

    public class ExampleView : AbstractView<ExampleViewModel, object>

You can get args from field `View.Args` / `ViewModel.Args` and set args with `SetArgs(TArgs args)` method or use special activation method `Activate(TArgs args)`

### You can override this methods

        protected override void OnInitialize() { }

        protected override void OnShutdown() { }

        protected override void OnActivate() { }

        protected override void OnUpdate(float deltaTime) { }

        protected override void OnDeactivate() { }

You can create and destroy nested dynamic views with using `CreateView` and `DestroyView` in your `View`

If you want to use a pool or any other way to create and destroy nested views, then override the `CreateViewInternal` and `DestroyViewInternal` methods in your `View`

If you want control creating ViewModel or you need use DI, then override `CreateViewModel` method in your `View`