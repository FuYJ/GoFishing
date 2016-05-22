using JsonFx.Json;

public class BluetoothDeviceInfo
{
	/// <summary>
	/// Device Name
	/// </summary>
	[JsonName("name")]
	public string Name { get; private set; }
	
	/// <summary>
	/// Mac address
	/// </summary>
	[JsonName("address")]
	public string Address { get; private set; }
	
	/// <summary>
	/// Type
	/// </summary>
	[JsonName("bluetoothClass")]
	public string BluetoothClass { get; private set; }
	
	/// <summary>
	/// Pairing status，10 = Unpaired、11 = Pairing、12 = Paired
	/// </summary>
	[JsonName("bondState")]
	public int BondState { get; private set; }
}
