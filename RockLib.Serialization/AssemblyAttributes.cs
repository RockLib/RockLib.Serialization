using RockLib.Configuration.ObjectFactory;
using RockLib.Serialization;
using System.Collections.Generic;

[assembly: ConfigSection("RockLib.Serialization:JsonSerializers", typeof(List<ISerializer>))]
[assembly: ConfigSection("RockLib.Serialization:XmlSerializers", typeof(List<ISerializer>))]
