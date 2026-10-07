# NeuralVis

> A neural network visualization framework built entirely from scratch in C# and .NET.

NeuralVis is a software engineering project focused on understanding how neural networks learn internally by implementing every major component from first principles instead of relying on machine learning libraries.

The project currently supports training and visualizing an XOR neural network and is actively being expanded into a handwritten digit classifier using the MNIST dataset.

---

# Motivation

Most machine learning frameworks abstract away the mathematical operations behind neural networks.

The goal of NeuralVis is to understand exactly how neural networks learn by implementing every layer of the learning process manually, including:

- Matrix mathematics
- Feedforward propagation
- Backpropagation
- Gradient descent
- Activation functions
- Loss functions
- Weight updates
- Live visualization of neuron activations

Rather than treating neural networks as a "black box," this project explores the algorithms that make them work.

---

# Current Features

### Neural Network

- Feedforward neural network
- Configurable hidden layers
- Sigmoid activation
- Mean Squared Error (MSE) loss
- Gradient descent optimization
- Adjustable learning rate
- Save and load trained models

### Mathematics

- Matrix multiplication
- Matrix addition
- Matrix subtraction
- Matrix transpose
- Element-wise operations
- Random weight initialization

### Visualization

- Live loss graph
- Live neuron activation visualization
- Training progress monitoring
- WPF desktop interface

### Training

- XOR dataset training
- Configurable epochs
- Training history
- Model persistence

---

# Technologies

- C#
- .NET
- WPF
- Object-Oriented Programming
- Linear Algebra
- Machine Learning Fundamentals
- Git

---

# Architecture

```
Training Data
      │
      ▼
NeuralNetwork
      │
      ▼
Layer
      │
      ▼
Matrix Operations
      │
      ▼
Activation Function
      │
      ▼
Loss Function
      │
      ▼
Backpropagation
      │
      ▼
Gradient Descent
      │
      ▼
Updated Weights
```

Project organization:

```
Matrix.cs
        │
Activation.cs
        │
Loss.cs
        │
Layer.cs
        │
NeuralNetwork.cs
        │
TrainingSample.cs
        │
WPF Visualization
```

---

# Current Project Status

## Completed

- Matrix library
- Feedforward neural network
- Backpropagation
- Gradient descent
- XOR training
- Weight saving/loading
- Live loss graph
- Live neuron activation visualization

## In Progress

- MNIST dataset support
- Multi-class classification
- Digit visualization
- Performance improvements

---

# Example Workflow

```
Input Data

↓

Forward Propagation

↓

Prediction

↓

Loss Calculation

↓

Backpropagation

↓

Weight Updates

↓

Repeat Until Converged
```

---

# Planned Features

- MNIST handwritten digit recognition
- 784-input neural network
- Softmax activation
- Cross-Entropy loss
- Mini-batch gradient descent
- Per-digit accuracy metrics
- Confusion matrix
- Better visualization tools
- Additional activation functions

For the complete development roadmap, see **ROADMAP.md**.

---

# Screenshots

*(To be added as development progresses.)*


# Running the Project

Clone the repository

```bash
git clone https://github.com/CodieneMonster/NeuralVis-Goggles.git
```

Open the solution in Visual Studio.

Build the solution.

Run the application.

---


---
