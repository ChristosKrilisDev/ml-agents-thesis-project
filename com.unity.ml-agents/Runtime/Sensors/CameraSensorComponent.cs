using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.MLAgents.Sensors
{
    /// <summary>
    /// A SensorComponent that creates a <see cref="CameraSensor"/>.
    /// </summary>
    [AddComponentMenu("ML Agents/Camera Sensor", (int)MenuGroup.Sensors)]
    public class CameraSensorComponent : SensorComponent, IDisposable
    {
        [HideInInspector] [SerializeField] [FormerlySerializedAs("camera")]
        private Camera m_Camera;

        private CameraSensor m_Sensor;

        /// <summary>
        /// Camera object that provides the data to the sensor.
        /// </summary>
        public Camera Camera
        {
            get => m_Camera;
            set
            {
                m_Camera = value;
                UpdateSensor();
            }
        }

        [HideInInspector] [SerializeField] [FormerlySerializedAs("sensorName")]
        private string m_SensorName = "CameraSensor";

        /// <summary>
        /// Name of the generated <see cref="CameraSensor"/> object.
        /// Note that changing this at runtime does not affect how the Agent sorts the sensors.
        /// </summary>
        public string SensorName
        {
            get => m_SensorName;
            set => m_SensorName = value;
        }

        [HideInInspector] [SerializeField] [FormerlySerializedAs("width")]
        private int m_Width = 84;

        /// <summary>
        /// Width of the generated observation.
        /// Note that changing this after the sensor is created has no effect.
        /// </summary>
        public int Width
        {
            get => m_Width;
            set => m_Width = value;
        }

        [HideInInspector] [SerializeField] [FormerlySerializedAs("height")]
        private int m_Height = 84;

        /// <summary>
        /// Height of the generated observation.
        /// Note that changing this after the sensor is created has no effect.
        /// </summary>
        public int Height
        {
            get => m_Height;
            set => m_Height = value;
        }

        [HideInInspector] [SerializeField] [FormerlySerializedAs("grayscale")]
        private bool m_Grayscale;

        /// <summary>
        /// Whether to generate grayscale images or color.
        /// Note that changing this after the sensor is created has no effect.
        /// </summary>
        public bool Grayscale
        {
            get => m_Grayscale;
            set => m_Grayscale = value;
        }

        [HideInInspector] [SerializeField]
        private ObservationType m_ObservationType;

        /// <summary>
        /// The type of the observation.
        /// </summary>
        public ObservationType ObservationType
        {
            get => m_ObservationType;
            set
            {
                m_ObservationType = value;
                UpdateSensor();
            }
        }

        [HideInInspector] [SerializeField]
        [Range(1, 50)]
        [Tooltip("Number of camera frames that will be stacked before being fed to the neural network.")]
        private int m_ObservationStacks = 1;

        [HideInInspector] [SerializeField] [FormerlySerializedAs("compression")]
        private SensorCompressionType m_Compression = SensorCompressionType.PNG;

        /// <summary>
        /// The compression type to use for the sensor.
        /// </summary>
        public SensorCompressionType CompressionType
        {
            get => m_Compression;
            set
            {
                m_Compression = value;
                UpdateSensor();
            }
        }

        /// <summary>
        /// Whether to stack previous observations. Using 1 means no previous observations.
        /// Note that changing this after the sensor is created has no effect.
        /// </summary>
        public int ObservationStacks
        {
            get => m_ObservationStacks;
            set => m_ObservationStacks = value;
        }

        /// <summary>
        /// Creates the <see cref="CameraSensor"/>
        /// </summary>
        /// <returns>The created <see cref="CameraSensor"/> object for this component.</returns>
        public override ISensor[] CreateSensors()
        {
            Dispose();
            m_Sensor = new CameraSensor(m_Camera, m_Width, m_Height, Grayscale, m_SensorName, m_Compression, m_ObservationType);

            if (ObservationStacks != 1)
            {
                return new ISensor[]
                {
                    new StackingSensor(m_Sensor, ObservationStacks)
                };
            }

            return new ISensor[]
            {
                m_Sensor
            };
        }

        /// <summary>
        /// Update fields that are safe to change on the Sensor at runtime.
        /// </summary>
        internal void UpdateSensor()
        {
            if (m_Sensor != null)
            {
                m_Sensor.Camera = m_Camera;
                m_Sensor.CompressionType = m_Compression;
            }
        }

        /// <summary>
        /// Clean up the sensor created by CreateSensors().
        /// </summary>
        public void Dispose()
        {
            if (!ReferenceEquals(m_Sensor, null))
            {
                m_Sensor.Dispose();
                m_Sensor = null;
            }
        }
    }
}
