# Octopus Role Mapper

## ver 1.0
Octopus Role Mapper allows to configure Octopus machine role mappings in configuration files,
YAML format, so that configuration can be put under source control as well as be automatically applied on Octopus.

Using the Octopus Role Mapper, together with source control system like git, gives the following benefits:
* change history,
* ability to use the previous version of configuration,
* ability to create a review process before applying changes.

## Build
To build project, execute a following command from Powershell:
`PS> .\build.ps1` 

## OctopusRoleMapper.Console.exe usage
The OctopusRoleMapper.Console.exe requires following paramters:

|Parameter|Description|
|---------|-----------|
|a:action|Action to perform: Upload or Download|
|d:definitions|Definitions directory|
|k:octopusApiKey|Octopus API key|
|u:octopusUrl|Octopus Url|

The **download** action allows to download the current Octopus roles mappings to the target directory,
the **upload** action allows to apply the yaml configuration on Octopus server.

## Examples

Mappings are stored in a files named as a corresponding role name eg : Role_rolename.yml
and has following structure

```Yaml
Name: myrole
Machines:
- Machine1
- Machine2
- Machine3
```

## Behavior

In case of **upload** action:

Machines on Octopus will be updated with roles defined in the definitions folder.
If machine is missing defined role it will be added.
If machine has known (we have definition for it in repository) role, 
but in repo it doesn't point to this particular machine - it will be removed.
However we do not touch unknown roles (roles we are not aware of eg: they are not defined in a repository)

## ver 2.0

Added configuration mapping for Tenants and TenantTags.
All logic and behavior is the same as it was with roles.
But there is one quite noticeable change, configuration format now is a bit different:

For roles:
```Yaml
---
Roles:
- Name: myrole
  Machines:
  - Machine1
  - Machine2
...
```
For Tenants
```Yaml
---
Tenants:
- Name: mytenant
  Machines:
  - local
...
```

For TenantTags
```Yaml
---
TenantTags:
- Name: qmt/uk
  Machines:
  - local
...
```
