import { NbMenuItem } from '@nebular/theme';

export const MENU_ITEMS: NbMenuItem[] = [
  {
    title: 'Admin',
    link: '/pages/infoserv',
    expanded: true,
    home: true,
    children: [
      {
        title: 'Config',
        expanded: false,
        children: [
          {
            title: 'Get Data',
            fragment: 'getConfigurationData',
          },
          {
            title: 'Get Entity',
            fragment: 'getConfigurationEntity',
          },
          {
            title: 'Get Search',
            fragment: 'getConfigurationSearch',
          },
        ],
      },
      {
        title: 'Reload',
        expanded: false,
        children: [
          {
            title: 'Config',
            fragment: 'reloadConfig',
          },
          {
            title: 'Segments',
            fragment: 'reloadSegments',
          },
          {
            title: 'All Segments',
            fragment: 'reloadAllSegments',
          }
        ],
      },
      {
        title: 'Search',
        expanded: false,
        children: [
          {
            title: 'Statistics',
            fragment: 'getSearchIndexStatistics',
          },
          {
            title: 'Rebuild Index',
            fragment: 'rebuildSearchIndex',
          },
          {
            title: 'Reindex Index',
            fragment: 'reindexSearchIndex',
          },
          {
            title: 'Sync Index',
            fragment: 'syncSearchIndex',
          },
        ],
      },
      {
        title: 'Other',
        expanded: true,
        children: [
          {
            title: 'Discovery',
            fragment: 'getDiscovery',
          },
          {
            title: 'Meta Data',
            fragment: 'getMetaData',
          },
          {
            title: 'Get Clients',
            fragment: 'getClients',
          },
        ],
      },
    ],
  }
];
