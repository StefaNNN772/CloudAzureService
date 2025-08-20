<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" name="StackOverflowProject" generation="1" functional="0" release="0" Id="f044a739-3d20-4312-ba45-5bb11429c033" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="StackOverflowProjectGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="HealthMonitoringService:AdminAlertEmails" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/StackOverflowProject/StackOverflowProjectGroup/LB:HealthMonitoringService:AdminAlertEmails" />
          </inToChannel>
        </inPort>
        <inPort name="HealthMonitoringService:HealthMonitoring" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/StackOverflowProject/StackOverflowProjectGroup/LB:HealthMonitoringService:HealthMonitoring" />
          </inToChannel>
        </inPort>
        <inPort name="HealthStatusService:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/StackOverflowProject/StackOverflowProjectGroup/LB:HealthStatusService:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="NotificationService:EmailNotify" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/StackOverflowProject/StackOverflowProjectGroup/LB:NotificationService:EmailNotify" />
          </inToChannel>
        </inPort>
        <inPort name="StackOverflowService:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/StackOverflowProject/StackOverflowProjectGroup/LB:StackOverflowService:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="HealthMonitoringService:AlertEmailsConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapHealthMonitoringService:AlertEmailsConnectionString" />
          </maps>
        </aCS>
        <aCS name="HealthMonitoringService:HealthCheckConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapHealthMonitoringService:HealthCheckConnectionString" />
          </maps>
        </aCS>
        <aCS name="HealthMonitoringService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapHealthMonitoringService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="HealthMonitoringServiceInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapHealthMonitoringServiceInstances" />
          </maps>
        </aCS>
        <aCS name="HealthStatusService:HealthCheckConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapHealthStatusService:HealthCheckConnectionString" />
          </maps>
        </aCS>
        <aCS name="HealthStatusService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapHealthStatusService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="HealthStatusServiceInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapHealthStatusServiceInstances" />
          </maps>
        </aCS>
        <aCS name="NotificationService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapNotificationService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="NotificationServiceInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapNotificationServiceInstances" />
          </maps>
        </aCS>
        <aCS name="StackOverflowService:DataConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapStackOverflowService:DataConnectionString" />
          </maps>
        </aCS>
        <aCS name="StackOverflowService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapStackOverflowService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="StackOverflowServiceInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/StackOverflowProject/StackOverflowProjectGroup/MapStackOverflowServiceInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:HealthMonitoringService:AdminAlertEmails">
          <toPorts>
            <inPortMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthMonitoringService/AdminAlertEmails" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:HealthMonitoringService:HealthMonitoring">
          <toPorts>
            <inPortMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthMonitoringService/HealthMonitoring" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:HealthStatusService:Endpoint1">
          <toPorts>
            <inPortMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthStatusService/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:NotificationService:EmailNotify">
          <toPorts>
            <inPortMoniker name="/StackOverflowProject/StackOverflowProjectGroup/NotificationService/EmailNotify" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:StackOverflowService:Endpoint1">
          <toPorts>
            <inPortMoniker name="/StackOverflowProject/StackOverflowProjectGroup/StackOverflowService/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapHealthMonitoringService:AlertEmailsConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthMonitoringService/AlertEmailsConnectionString" />
          </setting>
        </map>
        <map name="MapHealthMonitoringService:HealthCheckConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthMonitoringService/HealthCheckConnectionString" />
          </setting>
        </map>
        <map name="MapHealthMonitoringService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthMonitoringService/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapHealthMonitoringServiceInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthMonitoringServiceInstances" />
          </setting>
        </map>
        <map name="MapHealthStatusService:HealthCheckConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthStatusService/HealthCheckConnectionString" />
          </setting>
        </map>
        <map name="MapHealthStatusService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthStatusService/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapHealthStatusServiceInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthStatusServiceInstances" />
          </setting>
        </map>
        <map name="MapNotificationService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/StackOverflowProject/StackOverflowProjectGroup/NotificationService/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapNotificationServiceInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/StackOverflowProject/StackOverflowProjectGroup/NotificationServiceInstances" />
          </setting>
        </map>
        <map name="MapStackOverflowService:DataConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/StackOverflowProject/StackOverflowProjectGroup/StackOverflowService/DataConnectionString" />
          </setting>
        </map>
        <map name="MapStackOverflowService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/StackOverflowProject/StackOverflowProjectGroup/StackOverflowService/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapStackOverflowServiceInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/StackOverflowProject/StackOverflowProjectGroup/StackOverflowServiceInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="HealthMonitoringService" generation="1" functional="0" release="0" software="C:\Users\sef_s\Desktop\Cloud - Projekat\StackOverflowProjectSolution\StackOverflowProject\csx\Debug\roles\HealthMonitoringService" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="AdminAlertEmails" protocol="tcp" portRanges="10102" />
              <inPort name="HealthMonitoring" protocol="tcp" portRanges="10100" />
            </componentports>
            <settings>
              <aCS name="AlertEmailsConnectionString" defaultValue="" />
              <aCS name="HealthCheckConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;HealthMonitoringService&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringService&quot;&gt;&lt;e name=&quot;AdminAlertEmails&quot; /&gt;&lt;e name=&quot;HealthMonitoring&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;HealthStatusService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationService&quot;&gt;&lt;e name=&quot;EmailNotify&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;StackOverflowService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthMonitoringServiceInstances" />
            <sCSPolicyUpdateDomainMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthMonitoringServiceUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthMonitoringServiceFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="HealthStatusService" generation="1" functional="0" release="0" software="C:\Users\sef_s\Desktop\Cloud - Projekat\StackOverflowProjectSolution\StackOverflowProject\csx\Debug\roles\HealthStatusService" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="8080" />
            </componentports>
            <settings>
              <aCS name="HealthCheckConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;HealthStatusService&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringService&quot;&gt;&lt;e name=&quot;AdminAlertEmails&quot; /&gt;&lt;e name=&quot;HealthMonitoring&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;HealthStatusService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationService&quot;&gt;&lt;e name=&quot;EmailNotify&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;StackOverflowService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthStatusServiceInstances" />
            <sCSPolicyUpdateDomainMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthStatusServiceUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthStatusServiceFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="NotificationService" generation="1" functional="0" release="0" software="C:\Users\sef_s\Desktop\Cloud - Projekat\StackOverflowProjectSolution\StackOverflowProject\csx\Debug\roles\NotificationService" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="EmailNotify" protocol="tcp" portRanges="10101" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;NotificationService&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringService&quot;&gt;&lt;e name=&quot;AdminAlertEmails&quot; /&gt;&lt;e name=&quot;HealthMonitoring&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;HealthStatusService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationService&quot;&gt;&lt;e name=&quot;EmailNotify&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;StackOverflowService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/StackOverflowProject/StackOverflowProjectGroup/NotificationServiceInstances" />
            <sCSPolicyUpdateDomainMoniker name="/StackOverflowProject/StackOverflowProjectGroup/NotificationServiceUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/StackOverflowProject/StackOverflowProjectGroup/NotificationServiceFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="StackOverflowService" generation="1" functional="0" release="0" software="C:\Users\sef_s\Desktop\Cloud - Projekat\StackOverflowProjectSolution\StackOverflowProject\csx\Debug\roles\StackOverflowService" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="DataConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;StackOverflowService&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringService&quot;&gt;&lt;e name=&quot;AdminAlertEmails&quot; /&gt;&lt;e name=&quot;HealthMonitoring&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;HealthStatusService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationService&quot;&gt;&lt;e name=&quot;EmailNotify&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;StackOverflowService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/StackOverflowProject/StackOverflowProjectGroup/StackOverflowServiceInstances" />
            <sCSPolicyUpdateDomainMoniker name="/StackOverflowProject/StackOverflowProjectGroup/StackOverflowServiceUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/StackOverflowProject/StackOverflowProjectGroup/StackOverflowServiceFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="StackOverflowServiceUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="HealthStatusServiceUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="NotificationServiceUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="HealthMonitoringServiceUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="HealthMonitoringServiceFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="HealthStatusServiceFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="NotificationServiceFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="StackOverflowServiceFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="HealthMonitoringServiceInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="HealthStatusServiceInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="NotificationServiceInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="StackOverflowServiceInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="a4332d9c-4b6d-48ef-9a7a-a94fc839e7b2" ref="Microsoft.RedDog.Contract\ServiceContract\StackOverflowProjectContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="3da42e0b-8f01-4ae6-b5b8-2ddd540f0212" ref="Microsoft.RedDog.Contract\Interface\HealthMonitoringService:AdminAlertEmails@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthMonitoringService:AdminAlertEmails" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="dd6b6874-fd8e-4add-a87d-02c0172b13de" ref="Microsoft.RedDog.Contract\Interface\HealthMonitoringService:HealthMonitoring@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthMonitoringService:HealthMonitoring" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="13a7be98-a9dd-4f76-8829-c9aa8a6c00f2" ref="Microsoft.RedDog.Contract\Interface\HealthStatusService:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/StackOverflowProject/StackOverflowProjectGroup/HealthStatusService:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="228879c4-f482-47c2-89b1-589f108ecc0f" ref="Microsoft.RedDog.Contract\Interface\NotificationService:EmailNotify@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/StackOverflowProject/StackOverflowProjectGroup/NotificationService:EmailNotify" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="0f5238be-9860-4acf-bf9e-40a69a65bdc9" ref="Microsoft.RedDog.Contract\Interface\StackOverflowService:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/StackOverflowProject/StackOverflowProjectGroup/StackOverflowService:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>