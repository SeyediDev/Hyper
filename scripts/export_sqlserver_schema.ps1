param([Parameter(Mandatory=$true)][string]$OutputPath)
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Data
$configRoot=Join-Path $PSScriptRoot '../src/CustomerPortal/Hyper.CustomerPortal.Api'
$candidates=[Collections.Generic.List[string]]::new()
function Scan($node) {
    if ($node -is [string]) { if ($node -match '(?i)(server|data source)\s*=' -and $node -match '(?i)(database|initial catalog)\s*=') { $candidates.Add($node) }; return }
    if ($null -eq $node) { return }
    if ($node -is [Collections.IDictionary]) { foreach($item in $node.Values) { Scan $item }; return }
    if ($node -is [Collections.IEnumerable]) { foreach($item in $node) { Scan $item } }
}
foreach($file in @('appsettings.Development.json','appsettings.json')) { Scan (Get-Content -LiteralPath (Join-Path $configRoot $file) -Raw | ConvertFrom-Json -AsHashtable) }
$connection=$null
foreach($candidate in $candidates) {
    try {
        $builder=[System.Data.SqlClient.SqlConnectionStringBuilder]::new($candidate)
        if ($builder.InitialCatalog -ne 'Hyperyek') { continue }
        $builder["Connect Timeout"]=8
        $connection=[System.Data.SqlClient.SqlConnection]::new($builder.ConnectionString)
        $connection.Open()
        break
    } catch { Write-Output ("Connection attempt failed: {0}; {1}" -f $_.Exception.GetType().Name, $_.Exception.GetBaseException().GetType().Name); if ($connection) { $connection.Dispose(); $connection=$null } }
}
if (!$connection) { throw "No configured Hyperyek database connection could be opened ($($candidates.Count) candidates). Credentials not logged." }
function Query([string]$sql) {
    $command=$connection.CreateCommand(); $command.CommandText=$sql
    $reader=$command.ExecuteReader(); $items=[Collections.Generic.List[object]]::new()
    try { while($reader.Read()) { $row=[ordered]@{}; for($i=0;$i -lt $reader.FieldCount;$i++) { $row[$reader.GetName($i)]=if($reader.IsDBNull($i)){$null}else{$reader.GetValue($i)} }; $items.Add($row) } }
    finally { $reader.Close(); $command.Dispose() }
    return ,$items.ToArray()
}
try {
    $schema=[ordered]@{
        database='Hyperyek'; capturedAtUtc=[DateTime]::UtcNow.ToString('O')
        objects=Query "SELECT o.object_id,s.name AS [schema],o.name,o.type FROM sys.objects o JOIN sys.schemas s ON s.schema_id=o.schema_id WHERE o.type IN ('U','V') AND o.is_ms_shipped=0 ORDER BY s.name,o.name"
        columns=Query "SELECT c.object_id,c.column_id,c.name,t.name AS sql_type,c.max_length,c.precision,c.scale,c.is_nullable,c.is_identity,c.is_computed,c.collation_name,dc.name AS default_name,dc.definition AS default_sql,cc.definition AS computed_sql,cc.is_persisted,CONVERT(varchar(80),ic.seed_value) AS identity_seed,CONVERT(varchar(80),ic.increment_value) AS identity_increment FROM sys.columns c JOIN sys.objects o ON o.object_id=c.object_id JOIN sys.types t ON c.user_type_id=t.user_type_id LEFT JOIN sys.default_constraints dc ON dc.object_id=c.default_object_id LEFT JOIN sys.computed_columns cc ON cc.object_id=c.object_id AND cc.column_id=c.column_id LEFT JOIN sys.identity_columns ic ON ic.object_id=c.object_id AND ic.column_id=c.column_id WHERE o.type IN ('U','V') AND o.is_ms_shipped=0 ORDER BY c.object_id,c.column_id"
        indexes=Query "SELECT i.object_id,i.index_id,i.name,i.type,i.is_unique,i.is_primary_key,i.is_unique_constraint,i.has_filter,i.filter_definition,ic.column_id,ic.key_ordinal,ic.is_descending_key,ic.is_included_column FROM sys.indexes i JOIN sys.index_columns ic ON ic.object_id=i.object_id AND ic.index_id=i.index_id JOIN sys.objects o ON o.object_id=i.object_id WHERE o.type IN ('U','V') AND o.is_ms_shipped=0 AND i.index_id>0 ORDER BY i.object_id,i.index_id,ic.index_column_id"
        foreignKeys=Query "SELECT fk.name,fk.parent_object_id,fk.referenced_object_id,fk.delete_referential_action_desc,fk.update_referential_action_desc,fc.constraint_column_id,fc.parent_column_id,fc.referenced_column_id FROM sys.foreign_keys fk JOIN sys.foreign_key_columns fc ON fc.constraint_object_id=fk.object_id ORDER BY fk.parent_object_id,fk.name,fc.constraint_column_id"
        checks=Query "SELECT parent_object_id,name,definition FROM sys.check_constraints ORDER BY parent_object_id,name"
    }
    $path=$OutputPath
    $schema | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $path -Encoding utf8
    $samples=Query 'SELECT TOP (3) name FROM sys.tables ORDER BY name'
    Write-Output ("Connected to Hyperyek; objects={0}; columns={1}; sample table names={2}" -f $schema.objects.Count,$schema.columns.Count,(($samples | ForEach-Object name) -join ', '))
} finally { $connection.Dispose() }


