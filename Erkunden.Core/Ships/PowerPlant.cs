using System;
using System.Collections.Generic;
using Erkunden.Core.Equipment;
using Erkunden.Core.Utils;

namespace Erkunden.Core.Ships
{
	public class PowerPlant
	{
		protected List<IDevice> _devices = new List<IDevice>();
		protected List<ICapacitor> _capacitors = new List<ICapacitor>();

		public float MaximumCapacitance { get; private set; } = 0;
		public float MaximumOutput { get; private set; } = 0;
		public float CurrentConsumption { get; private set; } = 0;

		public float AvailablePower => MaximumOutput - CurrentConsumption;
		public float CapacitorPower { get; private set; } = 0;

		public IEnumerable<ICapacitor> Capacitors => _capacitors;
		public IEnumerable<IDevice> ConnectedDevices => _devices;

		public bool ConnectDevice(IDevice device)
		{
			if (CurrentConsumption + device.PowerDemand > MaximumOutput)
				return false;
			_devices.Add(device);
			device.IsOnline = true;
			device.PowerPlant = this;
			CurrentConsumption += device.PowerDemand;
			return true;
		}
		public void DisconnectDevice(IDevice device)
		{
			if (!_devices.Remove(device)) return;
			device.IsOnline = false;
			device.PowerPlant = null;
			CurrentConsumption += device.PowerDemand;
		}

		public void ConnectCapacitor(ICapacitor capacitor)
		{
			_capacitors.Add(capacitor);
			capacitor.PowerPlant = this;
			MaximumCapacitance += capacitor.Capacitance;
		}
		public void DisconnectCapacitor(ICapacitor capacitor)
		{
			if (!_capacitors.Remove(capacitor)) return;
			capacitor.PowerPlant = null;
			MaximumCapacitance -= capacitor.Capacitance;
		}

		public void PerformPulse(GameTime gameTime)
		{
			var power = (MaximumOutput - CurrentConsumption) * gameTime.ellapsed;
			CapacitorPower = Math.Min(MaximumCapacitance, CapacitorPower + power);
		}
	}
}
