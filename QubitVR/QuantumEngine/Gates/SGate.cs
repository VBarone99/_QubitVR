using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Complex = System.Numerics.Complex;
using MathNet.Numerics.LinearAlgebra;

/// <summary>
/// Quantum gate child class. Performs a 90 degree rotation about the Z-axis.
/// </summary>
public class SGate : Gate
{
    /// <summary>
    /// The qubit index in which the SGate will be applied to.
    /// </summary>
    public int m_targetIndex { get; }

    /// <summary>
    /// Constructs an XGate given the number of qubits and a qubit index to apply it to.
    /// </summary>
    /// <param name="qubits">Number of qubits in the system.</param>
    /// <param name="targetIndex">Qubit index to apply the XGate to.</param>
    public SGate(int qubits, int targetIndex)
    {
        // Ensure positive integer number of qubits
        if (qubits < 1)
        {
            Debug.LogError("Number of qubits must be a positive integer.");
            return;
        }

        // Check that the targetIndex is appropriate for the number of qubits in the system.
        if (targetIndex < 0 || targetIndex > qubits - 1)
        {
            Debug.LogError("Index provided to constructor SGate(int qubits, int targetIndex) is out of range. Index of " + targetIndex +
                " provided. Index in range [0, " + (qubits - 1) + "] expected.");
            return;
        }

        m_numQubits = qubits;
        m_targetIndex = targetIndex;
        m_type = "SGate";
        m_matrix = NthRootMatrix(1);
    }

    /// <summary>
    /// Construct an Nth root SGate.
    /// </summary>
    /// <param name="n">The root degree to take of the current gate matrix.
    /// n=2 -> square root of the current gate matrix. n=3 -> cube root of the current gate matrix. etc.</param>
    /// <returns>Completed Nth root SGate object</returns>
    public override Gate NthRootGate(int n)
    {
        // Ensure that NthRoot to be calculated is valid. Must be a positive integer.
        if (n < 1)
        {
            Debug.LogError("Must provide a positive integer for NthRootGate");
            return new Gate(m_numQubits, NthRootMatrix(1));
        }

        return new Gate(m_numQubits, NthRootMatrix(n));
    }

    /// <summary>
    /// Construct the Nth root matrix of the current gate.
    /// </summary>
    /// <param name="n">The root degree to take of the current gate matrix.
    /// n=2 -> square root of the current gate matrix. n=3 -> cube root of the current gate matrix. etc.</param>
    /// <returns>Nth root matrix of the SGate</returns>
    protected override Matrix<Complex> NthRootMatrix(int n)
    {
        // Ensure that NthRoot to be calculated is valid. Must be a positive integer.
        if (n < 1)
        {
            Debug.LogError("Must provide a positive integer for NthRootMatrix");
            // Return the proper size identity matrix if the 'n' is invalid.
            return Matrix<Complex>.Build.SparseIdentity((int)Math.Pow(2, m_numQubits), (int)Math.Pow(2, m_numQubits));
        }

        // =========================
        // Calculate Nth Root Matrix
        // =========================
        double theta = -(Math.PI / 2); // Potentially make positive to rotate the other way?
        Matrix<Complex> identity = Matrix<Complex>.Build.SparseIdentity(2, 2);
        Matrix<Complex> pauliZ = Matrix<Complex>.Build.Sparse(2, 2);
        pauliZ[0, 0] = new Complex(1, 0);
        pauliZ[1, 1] = new Complex(-1, 0);

        Complex product1 = new Complex(Math.Cos(-theta / (2 * n)), Math.Sin(-theta / (2 * n)));
        Complex product2 = new Complex(Math.Cos(theta / (2 * n)), Math.Sin(theta / (2 * n)));

        Matrix<Complex> A = identity.Multiply(new Complex(product1.Real * product2.Real, 0.0f));
        Matrix<Complex> B = pauliZ.Multiply(new Complex(0.0f, product1.Real * product2.Imaginary));
        Matrix<Complex> C = identity.Multiply(new Complex(0.0f, product1.Imaginary * product2.Real));
        Matrix<Complex> D = pauliZ.Multiply(new Complex(-product1.Imaginary * product2.Imaginary, 0.0f));
        Matrix<Complex> NthRootMatrix = A.Add(B).Add(C).Add(D);

        // =======================================================
        // Calculate Qubit System Matrix using the Nth Root Matrix
        // =======================================================
        // Set the initial value of m_matrix based on targetIndex. Initial value is
        // the NthRootXGate if the first index is the target index, identity otherwise.
        Matrix<Complex> NthRootMatrixInSystem;
        if (m_targetIndex == 0)
            NthRootMatrixInSystem = NthRootMatrix;
        else
            NthRootMatrixInSystem = identity;

        // For each additional qubit, kronecker product the current state with the NthRootXGate if that qubits index
        // is the target index; kronecker product with the identity matrix otherwise.
        for (int currentIndex = 1; currentIndex < m_numQubits; currentIndex++)
        {
            if (currentIndex == m_targetIndex)
                NthRootMatrixInSystem = NthRootMatrixInSystem.KroneckerProduct(NthRootMatrix); //pauliZ
            else
                NthRootMatrixInSystem = NthRootMatrixInSystem.KroneckerProduct(identity);
        }

        return NthRootMatrixInSystem;
    }
}