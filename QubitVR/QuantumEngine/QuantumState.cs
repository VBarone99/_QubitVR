using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Complex = System.Numerics.Complex;
using MathNet.Numerics.LinearAlgebra;

/// <summary>
/// Handles all quantum calculations for a system of qubits.
/// </summary>
[System.Serializable]
public class QuantumState
{
    /// <summary>
    /// Number of qubits in the system.
    /// </summary>
    [SerializeReference] private int m_numQubits;
    /// <summary>
    /// The matrix for the system. rows = 2^m_numQubits; cols = 1.
    /// </summary>
    [SerializeReference] private Matrix<Complex> m_matrix;
    /// <summary>
    /// Get the matrix of the QuantumState
    /// </summary>
    /// <returns>m_matrix: The matrix for the system. rows = 2^m_numQubits; cols = 1.</returns>
    public Matrix<Complex> GetMatrix() { return m_matrix; }
    public int GetNumQubits() { return m_numQubits; }

    /// <summary>
    /// Constructs a quantum state given the number of qubits.
    /// </summary>
    /// <param name="qubits">Number of qubits in the system.</param>
    public QuantumState(int qubits)
    {
        // Set field m_numQubits. Must be greater than 0 to initialize a quantum state.
        if (qubits <= 0)
        {
            Debug.LogError("QuantumState must be initialized with a positive integer number of qubits");
            m_numQubits = -1;
            m_matrix = null;
            return;
        }
        else
            m_numQubits = qubits;

        // Initialize sparse matrix to all zeros (2 rows, 1 column)
        Matrix<Complex> state_0 = Matrix<Complex>.Build.Sparse(2, 1);

        // Set first element to 1 to create the matrix for state 0
        state_0[0, 0] = new Complex(1, 0);

        // Set the QuantumState matrix to the state_0 matrix
        m_matrix = state_0;

        // For each additional qubit, kronecker product the current state with state_0 to get new state.
        // This implies that all qubits are initialized to state 0.
        for (int currentIndex = 0; currentIndex < m_numQubits - 1; currentIndex++)
            m_matrix = m_matrix.KroneckerProduct(state_0);
    }

    /// <summary>
    /// Constructs a quantum state from a state matrix. m_numQubits = log2(numRowsInMatrix).
    /// </summary>
    /// <param name="initialMatrix">The state matrix to construct a quantum state out of. Number of rows must be a power of 2. Number of cols must be 1.</param>
    public QuantumState (Matrix<Complex> initialMatrix)
    {
        // Ensure that only 1 column exists
        if (initialMatrix.ColumnCount != 1)
        {
            Debug.LogError("Parameter matrix must have 1 column");
            m_numQubits = -1; m_matrix = null; return;
        }

        // Ensure that there exists enough rows in the matrix to perform a power of 2 check
        if (initialMatrix.RowCount < 2)
        {
            Debug.LogError("Insufficient number of rows in parameter matrix to construct quantum state");
            m_numQubits = -1; m_matrix = null; return;
        }

        // Check if tempNumQubits is a power of 2. If not, then quantum state cannot be constructed
        int tempRowCount = initialMatrix.RowCount;
        while (tempRowCount > 1)
        {
            if (tempRowCount % 2 != 0)
            {
                Debug.LogError("Unable to contstruct a quantum state with given matrix. Number of rows in the input matrix must be a power of 2");
                m_numQubits = -1; m_matrix = null; return;
            }
            tempRowCount /= 2;
        }

        // If all checks pass, copy initialMatrix parameter to field m_matrix. m_numQubits = log2(rowCount).
        m_matrix = initialMatrix;
        m_numQubits = (int)Math.Log(initialMatrix.RowCount, 2.0);
    }

    /// <summary>
    /// Applies a gate to the current quantum state.
    /// </summary>
    /// <param name="gate">Gate to be applied to the system.</param>
    public void ApplyGate(Gate gate)
    {
        // Ensure that the size of the gate matrix is valid
        if (gate.GetMatrix().ColumnCount != m_matrix.RowCount)
        {
            Debug.LogError("Invalid gate size. Expected matrix with " + m_matrix.RowCount + " columns. User provided " +
                gate.GetMatrix().ColumnCount + " columns instead.");
            return;
        }
        m_matrix = gate.GetMatrix().Multiply(m_matrix);
    }

    /// <summary>
    /// Measure a given list of qubit indices in the Up/Down basis. This function will change the state of the system after each measurement.
    /// </summary>
    /// <param name="indices">List of qubit indices to measure. Qubit indexing starts at 0.</param>
    public void Measure(List<int> indices)
    {
        foreach (int index in indices)
        {
            // Skip over any index value that is not a valid qubit index
            if (index < 0 || index > m_numQubits-1)
                continue;

            // PiUp Projector
            Matrix<Complex> piUp = new PiUpProjector(m_numQubits, index).GetMatrix();
            // PiDown Projector
            Matrix<Complex> piDown = new PiDownProjector(m_numQubits, index).GetMatrix();
            // Resulting state if qubit[index] measures up
            Matrix<Complex> resultingStateUp = piUp.Multiply(m_matrix);
            // Resulting state if qubit[index] measures down
            Matrix<Complex> resultingStateDown = piDown.Multiply(m_matrix);

            // Calculate probability of qubit measuring up:
            // 1. Find the conjugate transpose of the current state
            Matrix<Complex> conjugateTransposed = m_matrix.ConjugateTranspose();
            // 2. Multiply with the resulting matrix if the qubit measured up
            Matrix<Complex> finalMatrix = conjugateTransposed.Multiply(resultingStateUp);
            // 3. Retrieve the single real value from the matrix. This is the expectation value of measuring up.
            double expectationValueUp = finalMatrix[0, 0].Real;

            // Generate random number to compare with the 'up' expectation value.
            float randomNumber = UnityEngine.Random.Range(0f, 1f);

            // Compare random number with 'up' expectation value.
            if (randomNumber > expectationValueUp)
            {
                // Measure Up
                m_matrix = resultingStateDown;

                // Normalize the resulting state matrix with expectationValueDown
                if (expectationValueUp > 0.00001)
                    m_matrix = m_matrix.Divide(Math.Sqrt(1- expectationValueUp));
            }
            else
            {
                // Measure Down
                m_matrix = resultingStateUp;

                // Normalize the resulting state matrix with expectationValueUp
                if (expectationValueUp > 0.00001)
                    m_matrix = m_matrix.Divide(Math.Sqrt(expectationValueUp));
            }
        }
    }

    /// <summary>
    /// Calculates the (X, Y, Z) expectation values for all qubits in the system.
    /// </summary>
    /// <returns>List of Vector3(X, Y, Z) expectation values.</returns>
    public Vector3 getExpectationValues(int index)
    {
        // Return from any index value that is not a valid qubit index
        if (index < 0 || index > m_numQubits - 1)
        {
            Debug.LogError("Index provided to method getExpectationValues(int index) is out of range. Index of " + index +
                " provided. Index in range [0, " + (m_numQubits - 1) + "] expected.");
            return new Vector3(10, 10, 10);
        }  

        // Container for the calculated (X, Y, Z) expectation value
        Vector3 expectationValue;

        // 1. Generate the conjugate transpose of the current state to calculate expectation values
        Matrix<Complex> conjugateTransposed = m_matrix.ConjugateTranspose();
        
        // 2. Generate observables for current system conditons (numQubits and qubit index we want to observe)
        XObservable xObservable = new XObservable(m_numQubits, index);
        YObservable yObservable = new YObservable(m_numQubits, index);
        ZObservable zObservable = new ZObservable(m_numQubits, index);

        // 3. Calculate x, y, and z expectation values and add to vector3
        expectationValue.x = (float)conjugateTransposed.Multiply(xObservable.GetMatrix().Multiply(m_matrix))[0, 0].Real;
        expectationValue.y = (float)conjugateTransposed.Multiply(yObservable.GetMatrix().Multiply(m_matrix))[0, 0].Real;
        expectationValue.z = (float)conjugateTransposed.Multiply(zObservable.GetMatrix().Multiply(m_matrix))[0, 0].Real;

        return expectationValue;
    }

    /// <summary>
    /// Determines if one quantum state is equal to another.
    /// </summary>
    /// <param name="otherQuantumState">Quantum state to compare to.</param>
    /// <returns>True if the quantum states are equal. False if the matrix or number of qubits are different.</returns>
    public bool Equals(QuantumState otherQuantumState)
    {
        double floatingPointThreshold = 0.01;
        Matrix<Complex> otherMatrix = otherQuantumState.GetMatrix();
        if (this.m_numQubits != otherQuantumState.GetNumQubits())
            return false;
        if (m_matrix.RowCount != otherMatrix.RowCount)
            return false;
        if (m_matrix.ColumnCount != otherMatrix.ColumnCount)
            return false;
        for (int row = 0; row < m_matrix.RowCount; row++)
            for (int col = 0; col < m_matrix.ColumnCount; col++)
            {
                if (Math.Abs(m_matrix[row, col].Real - otherMatrix[row, col].Real) > floatingPointThreshold)
                    return false;        
                if (Math.Abs(m_matrix[row, col].Imaginary - otherMatrix[row, col].Imaginary) > floatingPointThreshold)
                    return false;   
            }
        return true;
    }
}