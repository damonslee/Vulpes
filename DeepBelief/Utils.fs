﻿namespace DeepBelief

module Utils =

    open MathNet.Numerics.Distributions
    open MathNet.Numerics.LinearAlgebra.Double
    open MathNet.Numerics.LinearAlgebra.Generic

    let prepend value (vec : Vector<float>) = 
        vector [ yield! value :: (vec |> Vector.toList) ]

    let prependForBias : Vector<float> -> Vector<float> = prepend 1.0

    let toRows (M : Matrix<float>) = [0..(M.RowCount - 1)] |> List.map(fun i -> M.Row i)
    let toColumns (M : Matrix<float>) = [0..(M.ColumnCount - 1)] |> List.map(fun i -> M.Column i)
    let to2DList (M : Matrix<float>) = [0..(M.RowCount - 1)] |> List.map(fun i -> List.ofArray ((M.Row i).ToArray()))

    let sigmoid x = 1.0 / (1.0 + exp(-x))

    let transpose (M : Matrix<float>) = M.Transpose()

    let prependColumn column M =
        column :: toColumns M |> DenseMatrix.ofColumnVectors :> Matrix<float>

    // Taken from http://www.cs.toronto.edu/~hinton/absps/guideTR.pdf, Section 8.
    // The initial weights should have zero mean and 0.01 standard deviation.
    let gaussianDistribution = new Normal(0.0, 0.01)

    let initGaussianWeights nHidden nVisible =
        DenseMatrix.randomCreate nHidden nVisible gaussianDistribution;

    let sumOfRows M = M |> Matrix.sumRowsBy (fun _ row -> row)
    let sumOfSquaresMatrix M = M |> Matrix.fold (fun acc element -> acc + element * element) 0.0
    let sumOfSquares v = v |> Vector.toArray |> Array.sum

    // When applying this measure to a classification problem, 
    // where the output vectors must n - 1 zeroes and a single
    // one, it has the nice property that it evaluates to one
    // for the wrong guess, and zero for an incorrect guess.  So
    // dividing it by the set size gives the pecentage error of 
    // the test run.
    let error (target : Vector<_>) (output : Vector<_>) =
        (target - output |> sumOfSquares) / 2.0