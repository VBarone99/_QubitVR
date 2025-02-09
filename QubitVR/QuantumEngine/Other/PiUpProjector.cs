using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complex = System.Numerics.Complex;
using MathNet.Numerics.LinearAlgebra;

/// <summary>
/// Quantum projector represeting the up basis.
/// </summary>
public class PiUpProjector : Projector
{
    /// <summary>
    /// The qubit index in which the PiUpProjector will be calculated for, and thus projected on to.
    /// </summary>
    public int m_qubitIndex { get; }

    /// <summary>
    /// Constructs a PiUpProjector given the number of qubits and a qubit index to project on to.
    /// </summary>
    /// <param name="qubits">Number of qubits in the system.</param>
    /// <param name="upIndex">Qubit index to apply the up projector to.</param>
    public PiUpProjector(int qubits, int upIndex)
    {
        // Ensure positive integer number of qubits
        if (qubits < 1)
        {
            Debug.LogError("Number of qubits must be a positive integer.");
            return;
        }

        // Check that the upIndex is appropriate for the number of qubits in the system.
        if (upIndex < 0 || upIndex > qubits-1)
        {
            Debug.LogError("Index provided to constructor PiUpProjector(int qubits, int upIndex) is out of range. Index of " + upIndex +
                " provided. Index in range [0, " + (qubits - 1) + "] expected.");
            return;
        }

        // Set PiUpProjector local variables
        m_numQubits = qubits;
        m_qubitIndex = upIndex;
        m_type = "PiUp";

        // Generate matrix for the Pi-Up projector
        Matrix<Complex> identity = Matrix<Complex>.Build.SparseIdentity(2, 2);
        Matrix<Complex> piUp = Matrix<Complex>.Build.Sparse(2, 2);
        piUp[0, 0] = new Complex(1, 0);

        // Set the initial value of m_matrix based on upIndex. Initial value is
        // the Pi-Up projector if the first index is the upIndex, identity otherwise.
        if (upIndex == 0)
            m_matrix = piUp;
        else
            m_matrix = identity;

        // For each additional qubit, kronecker product the current state with the Pi-Up projector if that qubits index
        // is the upIndex; kronecker product with the identity matrix otherwise.
        for (int currentIndex = 1; currentIndex < m_numQubits; currentIndex++)
        {
            if (currentIndex == upIndex)
                m_matrix = m_matrix.KroneckerProduct(piUp);
            else
                m_matrix = m_matrix.KroneckerProduct(identity);
        }
    }
}