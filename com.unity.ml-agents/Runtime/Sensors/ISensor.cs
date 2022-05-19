using System;
using System.Collections.Generic;

namespace Unity.MLAgents.Sensors
{
    /// <summary>
    /// The Dimension property flags of the observations
    /// </summary>
    [Flags]
    public enum DimensionProperty
    {
        /// <summary>
        /// No properties specified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// No Property of the observation in that dimension. Observation can be processed with
        /// fully connected networks.
        /// </summary>
        None = 1,

        /// <summary>
        /// Means it is suitable to do a convolution in this dimension.
        /// </summary>
        TranslationalEquivariance = 2,

        /// <summary>
        /// Means that there can be a variable number of observations in this dimension.
        /// The observations are unordered.
        /// </summary>
        VariableSize = 4
    }

    /// <summary>
    /// The ObservationType enum of the Sensor.
    /// </summary>
    public enum ObservationType
    {
        /// <summary>
        /// Collected observations are generic.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Collected observations contain goal information.
        /// </summary>
        GoalSignal = 1
    }

    /// <summary>
    /// Sensor interface for generating observations.
    /// </summary>
    public interface ISensor
    {
        /// <summary>
        /// Returns a description of the observations that will be generated by the sensor.
        /// See <see cref="ObservationSpec"/> for more details, and helper methods to create one.
        /// </summary>
        /// <returns>An object describing the observation.</returns>
        ObservationSpec GetObservationSpec();

        /// <summary>
        /// Write the observation data directly to the <see cref="ObservationWriter"/>.
        /// Note that this (and  <see cref="GetCompressedObservation"/>) may
        /// be called multiple times per agent step, so should not mutate any internal state.
        /// </summary>
        /// <param name="writer">Where the observations will be written to.</param>
        /// <returns>The number of elements written.</returns>
        int Write(ObservationWriter writer);

        /// <summary>
        /// Return a compressed representation of the observation. For small observations,
        /// this should generally not be implemented. However, compressing large observations
        /// (such as visual results) can significantly improve model training time.
        /// </summary>
        /// <returns>Compressed observation.</returns>
        byte[] GetCompressedObservation();

        /// <summary>
        /// Update any internal state of the sensor. This is called once per each agent step.
        /// </summary>
        void Update();

        /// <summary>
        /// Resets the internal state of the sensor. This is called at the end of an Agent's episode.
        /// Most implementations can leave this empty.
        /// </summary>
        void Reset();

        /// <summary>
        /// Return information on the compression type being used. If no compression is used, return
        /// <see cref="CompressionSpec.Default()"/>.
        /// </summary>
        /// <returns>An object describing the compression used by the sensor.</returns>
        CompressionSpec GetCompressionSpec();

        /// <summary>
        /// Get the name of the sensor. This is used to ensure deterministic sorting of the sensors
        /// on an Agent, so the naming must be consistent across all sensors and agents.
        /// </summary>
        /// <returns>The name of the sensor.</returns>
        string GetName();
    }


    /// <summary>
    /// Helper methods to be shared by all classes that implement <see cref="ISensor"/>.
    /// </summary>
    public static class SensorExtensions
    {
        /// <summary>
        /// Get the total number of elements in the ISensor's observation (i.e. the product of the
        /// shape elements).
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public static int ObservationSize(this ISensor sensor)
        {
            var obsSpec = sensor.GetObservationSpec();
            var count = 1;
            for (var i = 0; i < obsSpec.Rank; i++)
            {
                count *= obsSpec.Shape[i];
            }

            return count;
        }
    }

    internal static class SensorUtils
    {
        internal static void SortSensors(List<ISensor> sensors)
        {
            // Use InvariantCulture to ensure consistent sorting between different culture settings.
            sensors.Sort((x, y) => string.Compare(x.GetName(), y.GetName(), StringComparison.InvariantCulture));
        }
    }
}
