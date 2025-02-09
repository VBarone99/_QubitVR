using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complex = System.Numerics.Complex;
using MathNet.Numerics.LinearAlgebra;

/// <summary>
/// Quantum observable for the expectation value along the Z axis.
/// </summary>
public class ZObservable : Observable
{
    /// <summary>
    /// The qubit index in which the ZObservable will be calculated for.
    /// </summary>
    public int m_qubitIndex { get; }

    /// <summary>
    /// Constructs a ZObservable given the number of qubits and a qubit index.
    /// </summary>
    /// <param name="qubits">Number of qubits in the system.</param>
    /// <param name="zIndex">Qubit index to create the observable from.</param>
    public ZObservable(int qubits, int zIndex)
    {
        // Ensure positive integer number of qubits
        if (qubits < 1)
        {
            Debug.LogError("Number of qubits must be a positive integer.");
            return;
        }

        // Check that the zIndex is appropriate for the number of qubits in the system.
        if (zIndex < 0 || zIndex > qubits - 1)
        {
            Debug.LogError("Index provided to constructor ZObservable(int qubits, int zIndex) is out of range. Index of " + zIndex +
                " provided. Index in range [0, " + (qubits - 1) + "] expected.");
            return;
        }

        // Set ZObservable local variables
        m_numQubits = qubits;
        m_qubitIndex = zIndex;
        m_type = "ZObservable";

        // Generate matrix for the Z Observable
        Matrix<Complex> identity = Matrix<Complex>.Build.SparseIdentity(2, 2);
        Matrix<Complex> pauliZ = Matrix<Complex>.Build.Sparse(2, 2);
        pauliZ[0, 0] = new Complex(1, 0);
        pauliZ[1, 1] = new Complex(-1, 0);

        // Set the initial value of m_matrix based on zIndex. Initial value is
        // the Z Observable if the first index is the zIndex, identity otherwise.
        if (zIndex == 0)
            m_matrix = pauliZ;
        else
            m_matrix = identity;

        // For each additional qubit, kronecker product the current state with the Z Observable if that qubits index
        // is the zIndex; kronecker product with the identity matrix otherwise.
        for (int currentIndex = 1; currentIndex < m_numQubits; currentIndex++)
        {
            if (currentIndex == zIndex)
                m_matrix = m_matrix.KroneckerProduct(pauliZ);
            else
                m_matrix = m_matrix.KroneckerProduct(identity);
        }
    }
}