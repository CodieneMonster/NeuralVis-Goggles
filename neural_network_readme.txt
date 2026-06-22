Neural Network From Scratch (C#)

This project implements a neural network from scratch in C# without using external machine learning libraries. 
The goal is to understand and build every component manually, 
including matrix operations, forward propagation, and eventually backpropagation and training.

Project Goal

Build a complete neural network system from the ground up, including:
- Matrix math engine
- Activation functions
- Loss functions
- Layer abstraction
- Neural network architecture
- Forward pass (completed)
- Backpropagation (in progress)
- Training loop
- Visualization (planned)

Development Approach

Matrix → Activation → Loss → Layer → NeuralNetwork → Backpropagation → Training

Implemented Components

Matrix Class:
- Random initialization
- Printing
- Addition and subtraction
- Scalar multiplication
- Transpose
- Matrix multiplication
- Element-wise multiplication
- Functional mapping

Activation Class:
- Sigmoid
- Sigmoid derivative
- Sigmoid derivative (from output)

Loss Class:
- Mean Squared Error
- MSE derivative

TrainingSample:
- Stores input and target matrices

Layer:
- weights, bias, z, a, input
- z = weights × input + bias
- a = sigmoid(z)

NeuralNetwork:
- hiddenLayer1, hiddenLayer2, outputLayer
- Forward pass chaining layers

Testing Completed:
- Matrix operations verified
- Activation verified
- Loss verified
- Layer forward pass verified
- Neural network tested on XOR inputs

Current Status:
- Forward pass complete
- Backpropagation in progress

Next Steps:
- Implement backpropagation
- Train network
- Add visualization

Design Principles:
- No external libraries
- Step-by-step learning
- Focus on correctness
- Deterministic testing first

Summary:
This project builds a neural network from first principles with a focus on deep understanding of each component.
