using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complex = System.Numerics.Complex;
using MathNet.Numerics.LinearAlgebra;

/// <summary>
/// Quantum observable for the expectation value along the Y axis.
/// </summary>
public class YObservable : Observable
{
    /// <summary>
    /// The qubit index in which the YObservable will be calculated for.
    /// </summary>
    public int m_qubitIndex { get; }

    /// <summary>
    /// Constructs a YObservable given the number of qubits and a qubit index.
    /// </summary>
    /// <param name="qubits">Number of qubits in the system.</param>
    /// <param name="yIndex">Qubit index to create the observable from.</param>
    public YObservable(int qubits, int yIndex)
    {
        // Ensure positive integer number of qubits
        if (qubits < 1)
        {
            Debug.LogError("Number of qubits must be a positive integer.");
            return;
        }

        // Check that the yIndex is appropriate for the number of qubits in the system.
        if (yIndex < 0 || yIndex > qubits - 1)
        {
            Debug.LogError("Index provided to constructor YObservable(int qubits, int yIndex) is out of range. Index of " + yIndex +
                " provided. Index in range [0, " + (qubits - 1) + "] expected.");
            return;
        }

        // Set YObservable local variables
        m_numQubits = qubits;
        m_qubitIndex = yIndex;
        m_type = "YObservable";

        // Generate matrix for the Y Observable
        Matrix<Complex> identity = Matrix<Complex>.Build.SparseIdentity(2, 2);
        Matrix<Complex> pauliY = Matrix<Complex>.Build.Sparse(2, 2);
        pauliY[0, 1] = new Complex(0, -1);
        pauliY[1, 0] = new Complex(0, 1);

        // Set the initial value of m_matrix based on upIndex. Initial value is
        // the Y Observable if the first index is the yIndex, identity otherwise.
        if (yIndex == 0)
            m_matrix = pauliY;
        else
            m_matrix = identity;

        // For each additional qubit, kronecker product the current state with the Y Observable if that qubits index
        // is the yIndex; kronecker product with the identity matrix otherwise.
        for (int currentIndex = 1; currentIndex < m_numQubits; currentIndex++)
        {
            if (currentIndex == yIndex)
                m_matrix = m_matrix.KroneckerProduct(pauliY);
            else
                m_matrix = m_matrix.KroneckerProduct(identity);
        }
    }
}