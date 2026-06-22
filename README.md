# NeuralVisualizer MNIST Perceptron Roadmap

## Project Goal

Turn the current XOR neural network visualizer into a handwritten digit recognition project using the MNIST dataset.

The new model should load handwritten digit images, train on MNIST, predict digits from `0` to `9`, and visualize how activations move through the network.

This project should stay consistent with the current workflow:

```text
Matrix -> Activation -> Loss -> Layer -> NeuralNetwork -> Training -> UI Visualization
```

## Uploaded Dataset Files

Two MNIST dataset packages are available:

### `MNIST_CSV.zip`

Use this first because CSV parsing is easier in C#.

Contains:

```text
mnist_train.csv
mnist_test.csv
generate_mnist_csv.py
readme.md
```

Expected CSV row format:

```text
label,pixel0,pixel1,pixel2,...,pixel783
```

Each image has:

```text
28 x 28 = 784 pixels
```

### `MNIST_ORG.zip`

Use later after the CSV version works.

Contains the original IDX files:

```text
train-images.idx3-ubyte
train-labels.idx1-ubyte
t10k-images.idx3-ubyte
t10k-labels.idx1-ubyte
```

## Important Design Change

The old XOR model used:

```text
2 inputs -> hidden layer 1 -> hidden layer 2 -> 1 output
```

MNIST needs:

```text
784 inputs -> hidden layer 1 -> hidden layer 2 -> 10 outputs
```

Why:

```text
784 inputs = one input per image pixel
10 outputs = one output neuron per digit class
```

Output meaning:

```text
output[0] = confidence for digit 0
output[1] = confidence for digit 1
...
output[9] = confidence for digit 9
```

The predicted digit is the index with the highest output value.

Example:

```text
Output = [0.01, 0.02, 0.91, 0.03, 0.00, 0.01, 0.01, 0.00, 0.00, 0.01]
Prediction = 2
```

## Assistant Collaboration Rules

Follow these rules strictly when helping with this project:

1. **Do not give full code unless explicitly asked**
   - Let me implement the solution myself.
   - Provide logic, structure, and steps.
   - Only provide full code when I explicitly request it.

2. **Teach concept first, then implementation**
   - Explain what we are doing conceptually.
   - Then explain how to implement it.

3. **Keep explanations clear and direct**
   - Avoid unnecessary theory.
   - Avoid overly complex math explanations.
   - Use structured, step-by-step reasoning.

4. **Focus on one step at a time**
   - Do not jump ahead.
   - Do not explain future systems prematurely.
   - Only explain what is required for the current step.

5. **Encourage reasoning**
   - Ask small checkpoint questions when appropriate.
   - Help verify my understanding before continuing.
7. **Help debug, do not just fix**
   - If I provide code, explain what is incorrect.
   - Explain why it is incorrect.
   - Guide me to the solution instead of rewriting everything.

8. **Stay consistent with the current workflow**
   - Build from the existing XOR network.
   - Do not randomly replace the architecture unless necessary.
   - Keep using the custom `Matrix`, `Layer`, `NeuralNetwork`, `TrainingSample`, and UI flow.

## Current Project State

The project already has:

```text
Matrix operations
Sigmoid activation
MSE loss
Layer forward pass
Backpropagation
Training loop
XOR training
Save/load weights
WPF UI
Loss history
Live loss graph
Live neuron activation graph
```

The next project phase is to convert the XOR model into an MNIST digit classifier.

---

# Full Task List

## Phase 1: Preserve the Current XOR Version

Goal: Avoid breaking the working XOR visualizer.

Tasks:

- [ ] Save a backup copy of the current working project.
- [ ] Keep XOR training working while MNIST code is added.
- [ ] Add a UI mode later for:
  - XOR mode
  - MNIST mode
- [ ] Keep the current live loss graph.
- [ ] Keep the current layer activation graph.
- [ ] Do not delete the XOR sample code yet.

Checkpoint:

```text
The current XOR model should still train and visualize correctly.
```

---

## Phase 2: Understand the MNIST Data Shape

Goal: Know exactly what the model input and output should look like.

Tasks:

- [ ] Confirm each MNIST image has 784 pixel values.
- [ ] Confirm each label is an integer from 0 to 9.
- [ ] Understand that each image input should become a `784 x 1` matrix.
- [ ] Understand that each target label should become a `10 x 1` one-hot vector.
- [ ] Normalize pixel values from `0-255` into `0.0-1.0`.

Input shape:

```text
784 x 1
```

Target shape:

```text
10 x 1
```

Example label conversion:

```text
label = 3

target =
[0]
[0]
[0]
[1]
[0]
[0]
[0]
[0]
[0]
[0]
```

Checkpoint question:

```text
Why does MNIST need 10 output neurons instead of 1?
```

---

## Phase 3: Create MNIST Training Sample Support

Goal: Reuse the current `TrainingSample` idea for MNIST.

Tasks:

- [ ] Confirm `TrainingSample` can already hold:
  - `Matrix Input`
  - `Matrix Target`
- [ ] Use the same class for MNIST.
- [ ] Create MNIST samples where:
  - `Input` is `784 x 1`
  - `Target` is `10 x 1`
- [ ] Add helper logic for one-hot encoding.
- [ ] Add helper logic for pixel normalization.

Data flow:

```text
CSV row
    -> label
    -> 784 pixel values
    -> normalized 784 x 1 input matrix
    -> one-hot 10 x 1 target matrix
    -> TrainingSample
```

Checkpoint:

```text
One MNIST CSV row should become one TrainingSample.
```

---

## Phase 4: Build an MNIST CSV Loader

Goal: Load `mnist_train.csv` and `mnist_test.csv`.

Tasks:

- [ ] Extract `MNIST_CSV.zip`.
- [ ] Locate `mnist_train.csv`.
- [ ] Locate `mnist_test.csv`.
- [ ] Create a new class:
  - `MnistCsvLoader`
- [ ] Add method conceptually like:
  - `LoadSamples(filePath, maxSamples)`
- [ ] Read the CSV file line by line.
- [ ] Split each row by comma.
- [ ] First value is the label.
- [ ] Remaining 784 values are pixels.
- [ ] Normalize each pixel:
  - `pixel / 255.0`
- [ ] Convert label into one-hot target.
- [ ] Return a `List<TrainingSample>`.

Important:

```text
Do not load all 60,000 training rows immediately at first.
```

Start small:

```text
100 samples
1000 samples
5000 samples
full dataset later
```

Checkpoint:

```text
Can the loader print the first label and confirm the input matrix is 784 x 1?
```

---

## Phase 5: Add MNIST Network Architecture

Goal: Create a network shape that supports digit classification.

Tasks:

- [ ] Keep XOR architecture available:
  - `2 -> 4 -> 4 -> 1`
- [ ] Add MNIST architecture:
  - `784 -> hidden1 -> hidden2 -> 10`
- [ ] Start with smaller hidden layers:
  - `784 -> 64 -> 32 -> 10`
- [ ] Later experiment with:
  - `784 -> 128 -> 64 -> 10`
- [ ] Confirm matrix shapes for each layer.

Shape example:

```text
Input: 784 x 1

Hidden layer 1:
weights = 64 x 784
bias    = 64 x 1
output  = 64 x 1

Hidden layer 2:
weights = 32 x 64
bias    = 32 x 1
output  = 32 x 1

Output layer:
weights = 10 x 32
bias    = 10 x 1
output  = 10 x 1
```

Checkpoint:

```text
For MNIST, why should the final output matrix be 10 x 1?
```

---

## Phase 6: Update Prediction Logic for Multi-Class Output

Goal: Replace binary threshold prediction with argmax prediction for MNIST.

Current XOR logic:

```text
if output >= 0.5:
    predicted label = 1
else:
    predicted label = 0
```

MNIST needs:

```text
predicted label = index of largest output value
```

Tasks:

- [ ] Add an argmax helper.
- [ ] Find the output neuron with the highest value.
- [ ] Convert one-hot target back into label using argmax.
- [ ] Update accuracy logic for 10 classes.
- [ ] Keep XOR accuracy logic separate or make prediction mode aware.

Example:

```text
output:
[0.01]
[0.04]
[0.88]
[0.02]
[0.01]
[0.00]
[0.02]
[0.01]
[0.00]
[0.01]

predicted digit = 2
```

Checkpoint:

```text
Why does binary threshold not work for 10-class digit recognition?
```

---

## Phase 7: Update Loss and Output Activation Strategy

Goal: Make training work better for 10 outputs.

Simple starting plan:

```text
Keep sigmoid output + MSE first
```

Why:

```text
The current network already supports sigmoid and MSE.
This makes the first MNIST version easier.
```

Tasks:

- [ ] Use 10 sigmoid output neurons first.
- [ ] Use MSE against the one-hot target first.
- [ ] Verify training runs without shape errors.
- [ ] Track whether loss decreases.
- [ ] Track whether accuracy improves.
- [ ] Later consider Softmax + Cross Entropy after the simple version works.

Do not start with softmax yet unless the current sigmoid/MSE version works.

Checkpoint:

```text
Can the model train for 1 epoch on 100 MNIST samples without crashing?
```

---

## Phase 8: Create MNIST Training Loop

Goal: Train on MNIST samples using the existing training pattern.

Tasks:

- [ ] Load a small training subset.
- [ ] Train one epoch at a time.
- [ ] Track average loss.
- [ ] Track accuracy.
- [ ] Add loss history points.
- [ ] Update UI status.
- [ ] Avoid refreshing huge UI tables too often.
- [ ] Do not show all MNIST prediction rows at once.

Suggested first training test:

```text
Training samples: 100
Testing samples: 20
Epochs: 1-5
Learning rate: small, such as 0.01 or 0.05
```

Checkpoint:

```text
Does loss decrease after a few epochs on a small sample?
```

---

## Phase 9: Add MNIST Prediction Rows

Goal: Show a small set of predictions in the UI.

Tasks:

- [ ] Create a new row class if needed:
  - `MnistPredictionRow`
- [ ] Store:
  - sample index
  - predicted digit
  - target digit
  - confidence
  - correct or incorrect
- [ ] Display only a small number of test predictions.
- [ ] Do not display thousands of rows.

Example row:

```text
Index: 17
Prediction: 8
Target: 8
Confidence: 0.74
Correct: true
```

Checkpoint:

```text
Can the UI show 10 test predictions after training?
```

---

## Phase 10: Add Image Visualization for MNIST

Goal: Show the actual 28x28 digit image.

Tasks:

- [ ] Convert a `784 x 1` input matrix back into a `28 x 28` grid.
- [ ] Draw pixels in WPF.
- [ ] Start with a simple grid or canvas.
- [ ] Pixel brightness should match normalized value.
- [ ] Add selected test sample display.
- [ ] Show:
  - image
  - target digit
  - predicted digit
  - confidence

Data flow:

```text
784 x 1 matrix
    -> 28 rows
    -> 28 columns
    -> pixel brightness
    -> drawn digit image
```

Checkpoint:

```text
Can I visually recognize the digit being shown?
```

---

## Phase 11: Update Activation Graph for MNIST

Goal: Reuse the neuron activation graph without drawing all 784 input neurons.

Problem:

```text
MNIST has 784 input neurons.
Drawing all 784 circles would be messy.
```

Tasks:

- [ ] Keep activation snapshot support.
- [ ] For MNIST, show compressed input visualization instead of 784 input circles.
- [ ] Show hidden layer activations normally.
- [ ] Show 10 output neurons for digits 0-9.
- [ ] Highlight the predicted output neuron.
- [ ] Label output neurons:
  - 0 through 9
- [ ] Show strongest output path if possible.

Suggested visualization:

```text
28x28 image -> Hidden 1 -> Hidden 2 -> Output digits 0-9
```

Checkpoint:

```text
Can the graph show which digit output neuron is most active?
```

---

## Phase 12: Save and Load MNIST Models

Goal: Save trained MNIST weights separately from XOR weights.

Tasks:

- [ ] Save architecture in the weight file.
- [ ] Save hidden layer weights and biases.
- [ ] Save output layer weights and biases.
- [ ] Use a different filename:
  - `mnist_weights.txt`
- [ ] Load model from file.
- [ ] Confirm predictions match before and after loading.
- [ ] Keep XOR save/load separate:
  - `xor_weights.txt`
  - `mnist_weights.txt`

Checkpoint:

```text
After loading mnist_weights.txt, does the model give the same prediction for the same image?
```

---

## Phase 13: UI Mode Switching

Goal: Let the app switch between XOR and MNIST.

Tasks:

- [ ] Add a mode property:
  - `CurrentMode`
- [ ] Add buttons:
  - `New XOR Model`
  - `Train XOR`
  - `Load XOR Model`
  - `New MNIST Model`
  - `Load MNIST Data`
  - `Train MNIST`
  - `Test MNIST`
- [ ] Hide or change UI sections depending on mode.
- [ ] Keep old XOR tools working.
- [ ] Add MNIST-specific sections:
  - digit image viewer
  - MNIST prediction rows
  - 10-output display

Checkpoint:

```text
Can I switch between XOR and MNIST without restarting the app?
```

---

## Phase 14: Performance Safety

Goal: Prevent the app from freezing or becoming too slow.

Tasks:

- [ ] Do not update UI every sample.
- [ ] Do not display thousands of rows.
- [ ] Use small sample counts during early testing.
- [ ] Use async training carefully.
- [ ] Add a cancel training option later.
- [ ] Add limits for:
  - epochs
  - learning rate
  - training sample count
  - test sample count
- [ ] Use `recordEvery` to control graph update frequency.
- [ ] Keep MNIST image drawing separate from training loop.

Recommended early limits:

```text
Epochs: 1 to 100
Learning rate: 0.0001 to 5.0
Training samples: 10 to 10000
Test samples: 10 to 1000
```

Checkpoint:

```text
Can the app train on 1000 samples without freezing?
```

---

## Phase 15: Later Improvements

Only do these after the basic MNIST version works.

Tasks:

- [ ] Add Softmax activation.
- [ ] Add Cross Entropy loss.
- [ ] Add mini-batch training.
- [ ] Shuffle training samples.
- [ ] Add better weight initialization.
- [ ] Add confusion matrix.
- [ ] Add per-digit accuracy.
- [ ] Add training cancellation.
- [ ] Add model metadata:
  - epochs trained
  - training samples used
  - accuracy
  - date saved
- [ ] Add original IDX loader using `MNIST_ORG.zip`.

Do not start these until:

```text
CSV loader works
MNIST model trains
Predictions display
Accuracy improves above random guessing
```

---

# New Chat Prompt

Use this prompt when starting a new ChatGPT conversation about this project:

```text
I am building a C# WPF neural network visualizer from scratch called NeuralVisualizer.

Current project state:
- I built a custom Matrix class.
- I built Activation, LossFunctions, Layer, TrainingSample, NeuralNetwork.
- I trained XOR using a 2 -> 4 -> 4 -> 1 network.
- I added backpropagation.
- I added save/load weights.
- I added a WPF UI.
- I added loss, accuracy, epochs, total epochs, editable learning rate, editable epochs.
- I added live loss history.
- I added a live loss graph using Canvas.
- I added a live neural activation graph for XOR using Canvas.
- I want to convert this into an MNIST digit classifier using the uploaded MNIST dataset.

Dataset files:
- MNIST_CSV.zip
  - mnist_train.csv
  - mnist_test.csv
  - generate_mnist_csv.py
  - readme.md
- MNIST_ORG.zip
  - train-images.idx3-ubyte
  - train-labels.idx1-ubyte
  - t10k-images.idx3-ubyte
  - t10k-labels.idx1-ubyte

Important design:
- XOR uses 2 inputs and 1 output.
- MNIST needs 784 inputs and 10 outputs.
- Each MNIST image is 28x28 pixels.
- Each input should be a 784 x 1 Matrix.
- Each label should be converted into a 10 x 1 one-hot target Matrix.
- Start with CSV loading first.
- Use sigmoid + MSE first because my existing network already supports it.
- Consider softmax + cross entropy later only after the basic version works.

How I want you to help:
1. Do not give full code unless I explicitly ask.
2. Teach concept first, then implementation.
3. Keep explanations clear and direct.
4. Focus on one step at a time.
5. Encourage reasoning and ask checkpoint questions.
6. Always explain matrix shapes and data flow.
7. Help debug by explaining what is wrong and why.
8. Stay consistent with my current workflow.

Start by helping me implement the MNIST CSV loader one step at a time.
```
