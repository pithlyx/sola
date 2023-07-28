$names = @("Compactor", "Conv0", "Conv1", "Conv2", "Conv3", "Conv4", "Crusher", "Cutter", "ChipFac0", "ChipFac1", "FuelFac", "Furnace", "Generator0", "Generator1", "Hub", "PowerStation", "Research0", "Research1", "Research2", "Saw", "Solar", "Storage", "Tunnel0", "Tunnel1")

foreach ($name in $names) {
    $content = @"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class $name : Building
{

}
"@

    Set-Content -Path "$name.cs" -Value $content
}
