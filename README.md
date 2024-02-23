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

Also you can args for view and viewModel. To do this, inherit from AbstractViewModel<Targs> and AbstractView<TViewModel, TArgs>

    public class ExampleViewModel : AbstractViewModel<object>

    public class ExampleView : AbstractView<ExampleViewModel, object>
