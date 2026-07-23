# NeuralVisualizer MNIST Roadmap

## Project Goal

NeuralVisualizer is a C# / WPF neural network visualizer built from scratch. The project started as an XOR neural network visualizer and is now being expanded into an MNIST handwritten digit classifier.

The main goal is to understand how a neural network learns by building the core pieces manually instead of relying on machine learning libraries. This includes matrix math, forward propagation, backpropagation, loss calculation, accuracy testing, and visual feedback through a desktop interface.

---

## Current Project Status

### Completed

* Built a custom neural network system in C#.
* Implemented matrix-based forward propagation.
* Implemented backpropagation and weight updates.
* Built a WPF interface for training and visualization.
* Added loss tracking through `LossHistory`.
* Added MNIST CSV loading.
* Converted MNIST rows into `784 x 1` input matrices.
* Converted digit labels into `10 x 1` one-hot target matrices.
* Added an MNIST network architecture:

```text
784 input neurons -> 64 hidden neurons -> 32 hidden neurons -> 10 output neurons
```

* Added digit prediction using `ArgMax`.
* Added MNIST image viewer that redraws the current `28 x 28` digit.
* Added separate training and testing datasets.
* Added test accuracy tracking.
* Added full training and one-epoch training modes.
* Removed old XOR-specific ViewModel code.

---

## Current Training Setup

The current MNIST setup uses:

```text
Training samples: 1000
Test samples: 200
Epochs: 5-10 for testing
Learning rate: 0.05
Activation: Sigmoid
Loss: Mean Squared Error
Output selection: ArgMax
```

Current accuracy is measured on the test set, not only on the training set. This makes the accuracy more meaningful because the model is judged on images it did not directly train on.

---

## Phase 1: Stabilize MNIST Training

### Goal

Make sure the training loop works correctly and consistently before adding more features.

### Tasks

* [x] Load MNIST training data from CSV.
* [x] Load MNIST testing data from CSV.
* [x] Train on `mnistSamples`.
* [x] Test accuracy on `mnistTestSamples`.
* [x] Update loss history.
* [x] Update predicted digit.
* [x] Update digit viewer.
* [ ] Confirm loss decreases over multiple epochs.
* [ ] Confirm test accuracy improves gradually.
* [ ] Confirm the network does not reset between training clicks.
* [ ] Confirm full training and one-epoch training behave consistently.

---

## Phase 2: Performance Testing

### Goal

Find out what is slowing down training.

Current training is slower than expected, so the next step is profiling the training loop.

### Tasks

* [ ] Measure time per epoch using `Stopwatch`.
* [ ] Test training with no accuracy calculation.
* [ ] Test training with no image updates.
* [ ] Test training with no graph updates.
* [ ] Compare speed between:

  * 100 training samples
  * 1000 training samples
  * 5000 training samples
* [ ] Identify whether the bottleneck is:

  * matrix math
  * backpropagation
  * accuracy testing
  * UI redraws
  * loss graph updates

### Target

The project should be able to train on 1000 MNIST samples for multiple epochs without feeling extremely slow.

---

## Phase 3: Improve Training Loop Efficiency

### Goal

Keep the UI responsive while avoiding unnecessary work every epoch.

### Planned Improvements

* [ ] Calculate test accuracy every 5 or 10 epochs instead of every epoch.
* [ ] Update the digit viewer every 5 or 10 epochs instead of every epoch.
* [ ] Keep loss tracking every epoch.
* [ ] Replace unnecessary `Task.Delay` usage with `Task.Yield` where possible.
* [ ] Avoid expensive UI redraws during every training step.
* [ ] Keep training work inside `Task.Run` so the WPF UI does not freeze.

### Recommended Training Flow

```text
For each epoch:
    Train one epoch
    Record loss
    Every few epochs:
        Calculate test accuracy
        Update digit viewer
        Update prediction display
```

---

## Phase 4: Improve Accuracy

### Goal

Increase test accuracy while still keeping the code understandable.

### Possible Improvements

* [ ] Shuffle training samples before each epoch.
* [ ] Try different learning rates:

  * `0.01`
  * `0.03`
  * `0.05`
  * `0.1`
* [ ] Try larger hidden layers:

```text
784 -> 128 -> 64 -> 10
```

* [ ] Compare against the current architecture:

```text
784 -> 64 -> 32 -> 10
```

* [ ] Train on more samples:

  * 1000
  * 5000
  * 10000
  * eventually 60000
* [ ] Add better initialization if needed.
* [ ] Later, replace Sigmoid + MSE with Softmax + Cross Entropy.

---

## Phase 5: MNIST Output Visualization

### Goal

Show more than just the predicted digit.

The network outputs 10 values, one for each possible digit. The UI should eventually show all 10 output neurons.

### Planned UI Features

* [ ] Show output value for digit `0`.
* [ ] Show output value for digit `1`.
* [ ] Show output value for digit `2`.
* [ ] Show output value for digit `3`.
* [ ] Show output value for digit `4`.
* [ ] Show output value for digit `5`.
* [ ] Show output value for digit `6`.
* [ ] Show output value for digit `7`.
* [ ] Show output value for digit `8`.
* [ ] Show output value for digit `9`.
* [ ] Highlight the largest output value.
* [ ] Display the target digit next to the predicted digit.
* [ ] Show whether the prediction was correct or incorrect.

Example:

```text
Target: 5
Prediction: 5
Correct: Yes

0: 0.04
1: 0.02
2: 0.10
3: 0.14
4: 0.01
5: 0.91
6: 0.03
7: 0.07
8: 0.18
9: 0.05
```

---

## Phase 6: Save and Load MNIST Models

### Goal

Allow the model to remember training after the app closes.

Right now, the neural network remembers learning only while the program is running. If the program closes, the weights are lost unless model saving is added.

### Tasks

* [ ] Save MNIST model weights.
* [ ] Save MNIST model biases.
* [ ] Load saved MNIST model.
* [ ] Add a "Save MNIST Model" button.
* [ ] Add a "Load MNIST Model" button.
* [ ] Display model status in the UI.
* [ ] Prevent loading models with incompatible architecture sizes.

---

## Phase 7: Clean Up the Codebase

### Goal

Make the project easier to maintain now that it is MNIST-focused.

### Tasks

* [ ] Remove old XOR comments.
* [ ] Remove unused imports.
* [ ] Remove old debug-only methods.
* [ ] Rename methods that still sound temporary.
* [ ] Separate MNIST logic into cleaner classes.
* [ ] Keep the ViewModel focused on application state.
* [ ] Keep drawing logic inside `MainWindow.xaml.cs`.
* [ ] Keep neural network logic inside the core neural network classes.

### Possible Folder Structure

```text
NeuralVisualizer.Core/
    Matrix.cs
    Layer.cs
    NeuralNetwork.cs
    Activation.cs
    LossFunctions.cs
    TrainingSample.cs

NeuralVisualizer.Core/Data/
    MnistCsvLoader.cs

NeuralVisualizer1/
    MainWindow.xaml
    MainWindow.xaml.cs
    MainViewModel.cs
    TrainingHistoryPoint.cs
```

---

## Phase 8: Better Dataset Controls

### Goal

Make dataset size configurable from the UI instead of changing code manually.

### Planned Features

* [ ] Input box for number of training samples.
* [ ] Input box for number of test samples.
* [ ] Button to reload MNIST data.
* [ ] Display current dataset size.
* [ ] Prevent training if no data is loaded.
* [ ] Display file loading errors clearly.

Example UI values:

```text
Training Samples: 1000
Test Samples: 200
Epochs: 10
Learning Rate: 0.05
```

---

## Phase 9: Better Training Metrics

### Goal

Track more information about the model's learning progress.

### Planned Metrics

* [ ] Training loss.
* [ ] Test accuracy.
* [ ] Total epochs trained.
* [ ] Current epoch.
* [ ] Correct predictions.
* [ ] Incorrect predictions.
* [ ] Time per epoch.
* [ ] Total training time.
* [ ] Best test accuracy reached.
* [ ] Loss trend over time.

---

## Phase 10: Future Advanced Improvements

These are not needed right now, but they are possible future upgrades.

### Better Neural Network Training

* [ ] Mini-batch training.
* [ ] Softmax output layer.
* [ ] Cross-entropy loss.
* [ ] ReLU activation.
* [ ] Adam optimizer.
* [ ] Better weight initialization.

### Better Visualizations

* [ ] Output neuron bar chart.
* [ ] Prediction confidence display.
* [ ] Correct/incorrect prediction highlighting.
* [ ] Side-by-side training and test examples.
* [ ] Confusion matrix for digits `0-9`.

### Performance Improvements

* [ ] Optimize matrix operations.
* [ ] Reduce temporary matrix allocations.
* [ ] Profile forward pass and backpropagation.
* [ ] Add optional GPU acceleration later.
* [ ] Compare CPU training speed vs GPU training speed.

### Future Project Extensions

* [ ] Hand-drawn digit input.
* [ ] User can draw a digit in the app.
* [ ] Model predicts the user's drawing.
* [ ] OCR-style character recognition.
* [ ] Eventually experiment with license plate character recognition in controlled examples.

---

## Development Priorities

### Immediate Priority

Make MNIST training fast enough and reliable enough for 1000 training samples and 200 test samples.

### Next Priority

Improve the training loop so accuracy and image updates do not slow training too much.

### Later Priority

Add model saving/loading and better output neuron visualization.

---

## Current Best Test Plan

Use this setup while debugging:

```text
Training samples: 1000
Test samples: 200
Epochs: 10
Learning rate: 0.05
Accuracy check: every 5 epochs
Image update: every 5 epochs
```

Expected behavior:

```text
Loss should decrease.
Test accuracy should increase over time.
The app should not freeze.
The image viewer should update.
The model should not reset between training clicks.
```

---

## Notes

This project is meant to show learning visually, not just produce the highest possible MNIST accuracy. The goal is to understand each part of the neural network system by building it manually.

Accuracy matters, but the bigger goal is understanding:

