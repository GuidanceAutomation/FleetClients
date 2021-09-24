using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FleetClients.Core
{
	public static class FleetManagerClient_ExtensionMethods
	{
		public enum DockingFault
		{
			Good = 0,
			Timeout = 1,
			LostPayload = 2,
			ChargeError = 3
		}

		/// <summary>
		/// This method queries for what the state of a given controller's ChargeError flag. This only accounts for that flag,
		/// and only on a KingpinVehicle-type vehicle
		/// </summary>
		/// <param name="vehicleIpAddress">The IPv4 address of the vehicle to query</param>
		/// <returns>If true, there is a charge error being reported</returns>
		public static bool GetChargeFaultFlagForKingpinVehicleAgent(this IFleetManagerClient client, IPAddress vehicleIpAddress)
		{
			bool result = false;

			var currentFleetState = client.FleetState.KingpinStates.ToArray(); // Make a copy of the fleet state to prevent any modification problems

			var vehicle = currentFleetState.FirstOrDefault(e => e.IPAddress == vehicleIpAddress);

			if (vehicle != null)
			{
				byte dockingFaultByte = vehicle.StateCastExtendedData[3];

				if ((DockingFault)dockingFaultByte == DockingFault.ChargeError)
					result = true;
			}

			return result;
		}
	}
}
