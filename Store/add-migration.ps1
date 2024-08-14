[CmdletBinding()]
param (
    [Parameter(mandatory=$true)]
    [String]
    $Name
)

function Add-Migration {
    param (
        [Parameter(mandatory=$true)]
        [String]
        $Context
    )

    #$output = Join-Path $PSScriptRoot "PerfumeStore.DatabaseMigration\Migrations"
    
    $migrationName = $Name + "_" + $context;

    dotnet ef migrations add $migrationName `
        --project "PerfumeStore.DatabaseMigration\PerfumeStore.DatabaseMigration.csproj" `
        --context $context `
}

Add-Migration -Context "AspNetIdentityDbContext"

Add-Migration -Context "ConfigurationDbContext"

Add-Migration -Context "PersistedGrantDbContext"

Add-Migration -Context "ShopDbContext"

