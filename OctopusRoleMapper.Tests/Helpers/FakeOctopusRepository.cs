﻿using Octopus.Client;
using Octopus.Client.Repositories;

namespace OctopusRoleMapper.Tests.Helpers
{
    class FakeOctopusRepository : IOctopusRepository
    {
        public FakeOctopusRepository()
        {
            MachineRoles = FakeMachineRoles = new FakeMachineRoleRepository();
            Machines = new FakeMachineRepository();
            TagSets = FakeTenantTags = new FakeTagSetsRepository();
            Tenants = FakeTenants = new FakeTenantsRepository();
        }

        public IOctopusClient Client { get; }
        public IArtifactRepository Artifacts { get; }
        public IActionTemplateRepository ActionTemplates { get; }
        public ICertificateRepository Certificates { get; }
        public IBackupRepository Backups { get; }
        public IBuiltInPackageRepositoryRepository BuiltInPackageRepository { get; }
        public IDashboardConfigurationRepository DashboardConfigurations { get; }
        public IDashboardRepository Dashboards { get; }
        public IDeploymentProcessRepository DeploymentProcesses { get; }
        public IDeploymentRepository Deployments { get; }
        public IEnvironmentRepository Environments { get; }
        public IEventRepository Events { get; }
        public IFeaturesConfigurationRepository FeaturesConfiguration { get; }
        public IFeedRepository Feeds { get; }
        public IInterruptionRepository Interruptions { get; }
        public ILibraryVariableSetRepository LibraryVariableSets { get; }
        public ILifecyclesRepository Lifecycles { get; }
        public IMachineRepository Machines { get; }
        public IMachineRoleRepository MachineRoles { get; }
        public IMachinePolicyRepository MachinePolicies { get; }
        public IProjectGroupRepository ProjectGroups { get; }
        public IProjectRepository Projects { get; }
        public IReleaseRepository Releases { get; }
        public IProxyRepository Proxies { get; }
        public IServerStatusRepository ServerStatus { get; }
        public ISchedulerRepository Schedulers { get; }
        public ISubscriptionRepository Subscriptions { get; }
        public ITaskRepository Tasks { get; }
        public ITeamsRepository Teams { get; }
        public ITagSetRepository TagSets { get; }
        public ITenantRepository Tenants { get; }
        public IUserRepository Users { get; }
        public IUserRolesRepository UserRoles { get; }
        public IVariableSetRepository VariableSets { get; }
        public IProjectTriggerRepository ProjectTriggers { get; }
        public IChannelRepository Channels { get; }
        public IAccountRepository Accounts { get; }
        public IRetentionPolicyRepository RetentionPolicies { get; }
        public IDefectsRepository Defects { get; }
        public IOctopusServerNodeRepository OctopusServerNodes { get; }
        public FakeMachineRoleRepository FakeMachineRoles { get; }
        public FakeTagSetsRepository FakeTenantTags { get; }
        public FakeTenantsRepository FakeTenants { get; }
    }
}
