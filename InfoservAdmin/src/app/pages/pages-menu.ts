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
            data: 'getConfigurationData',
          },
          {
            title: 'Get Entity',
            data: 'getConfigurationEntity',
          },
          {
            title: 'Get Search',
            data: 'getConfigurationSearch',
          },
        ],
      },
      {
        title: 'Reload',
        expanded: true,
        children: [
          {
            title: 'Config',
            data: 'reloadConfig',
          },
          {
            title: 'Segments',
            data: 'reloadSegments',
          },
          {
            title: 'All Segments',
            data: 'reloadAllSegments',
          }
        ],
      },
      {
        title: 'Search',
        expanded: false,
        children: [
          {
            title: 'Rebuild Index',
            data: 'rebuildSearchIndex',
          },
          {
            title: 'Reindex Index',
            data: 'reindexSearchIndex',
          },
          {
            title: 'Sync Index',
            data: 'syncSearchIndex',
          },
          {
            title: 'Statistics',
            data: 'getSearchIndexStatistics',
          },
        ],
      },
      {
        title: 'Other',
        expanded: true,
        children: [
          {
            title: 'Meta Data',
            data: 'getMetaData',
          },
          {
            title: 'Discovery',
            data: 'getDiscovery',
          },
          {
            title: 'Get Clients',
            data: 'getClients',
          },
        ],
      },
    ],
  },
  {
    title: 'Infoserv API',
    expanded: true,
    children: [
      {
        title: 'Data',
        data: 'getData,Data',
      },
      {
        title: 'Details',
        data: 'getDetails,Details',
      },
      {
        title: 'Segment Search',
        data: 'searchSegments,SegmentSearch',
      },
    ],
  },
  // {
  //   title: 'Data Analytics (Beta)',
  //   expanded: true,
  //   children: [
  //     // {
  //     //   title: '',
  //     //   data: 'getKibana1',
  //     //   url: 'http://localhost:5601/app/kibana#/visualize/edit/8f4d0c00-4c86-11e8-b3d7-01146121b73d?_g=()&_a=(filters:!(),linked:!f,query:(language:lucene,query:""),uiState:(vis:(legendOpen:!f)),vis:(aggs:!((enabled:!t,id:"1",params:(),schema:metric,type:count),(enabled:!t,id:"2",params:(field:Carrier,missingBucket:!f,missingBucketLabel:Missing,order:desc,orderBy:"1",otherBucket:!f,otherBucketLabel:Other,size:5),schema:segment,type:terms)),params:(addLegend:!t,addTooltip:!t,isDonut:!t,labels:(last_level:!t,show:!t,truncate:100,values:!t),legendPosition:right,type:pie),title:"%5BFlights%5D%20Airline%20Carrier",type:pie))',
  //     //   target: '_new',
  //     // },
  //     {
  //       title: 'API Usage (%) Last 30 Days',
  //       data: 'apiUsageLast30DaysPercent',
  //       url: 'http://localhost:5601/app/kibana#/visualize/create?type=pie&indexPattern=88ba8cf0-1e68-11e9-af33-f7dbce9cb9e9&_g=(refreshInterval:(display:Off,pause:!f,value:0),time:(from:now-30d,mode:quick,to:now))&_a=(filters:!(),linked:!f,query:(language:lucene,query:%27%27),uiState:(vis:(legendOpen:!t)),vis:(aggs:!((enabled:!t,id:%271%27,params:(customLabel:Metrics),schema:metric,type:count),(enabled:!t,id:%272%27,params:(field:Url.keyword,missingBucket:!f,missingBucketLabel:Missing,order:desc,orderBy:%271%27,otherBucket:!f,otherBucketLabel:Other,size:5),schema:segment,type:terms)),params:(addLegend:!t,addTooltip:!t,isDonut:!t,labels:(last_level:!t,show:!t,truncate:100,values:!t),legendPosition:left,type:pie),title:%27New%20Visualization%27,type:pie))',
  //       //url: 'http://elasticsearch-data00.corp.showingtime.net:5601/app/kibana#/visualize/create?type=pie&indexPattern=53598920-fc9a-11e8-9936-9df9677cd5df&_g=(refreshInterval:(display:Off,pause:!f,value:0),time:(from:now-30d,mode:quick,to:now))&_a=(filters:!(),linked:!f,query:(language:lucene,query:%27%27),uiState:(vis:(legendOpen:!t)),vis:(aggs:!((enabled:!t,id:%271%27,params:(),schema:metric,type:count),(enabled:!t,id:%272%27,params:(field:Url.keyword,size:10),schema:segment,type:significant_terms)),params:(addLegend:!t,addTooltip:!t,isDonut:!t,labels:(last_level:!t,show:!t,truncate:100,values:!t),legendPosition:left,type:pie),title:%27New%20Visualization%27,type:pie))',
  //       target: '_new',
  //     },
  //     {
  //       title: 'API Usage (Time) Last 30 Days',
  //       data: 'apiUsageLast30DaysTime',
  //       url: 'http://localhost:5601/app/kibana#/visualize/create?type=pie&indexPattern=88ba8cf0-1e68-11e9-af33-f7dbce9cb9e9&_g=(refreshInterval:(display:Off,pause:!f,value:0),time:(from:now-30d,mode:quick,to:now))&_a=(filters:!(),linked:!f,query:(language:lucene,query:%27%27),uiState:(),vis:(aggs:!((enabled:!t,id:%271%27,params:(customLabel:Duration,field:DurationMs),schema:metric,type:sum),(enabled:!t,id:%272%27,params:(field:Url.keyword,missingBucket:!f,missingBucketLabel:Missing,order:desc,orderBy:%271%27,otherBucket:!f,otherBucketLabel:Other,size:6),schema:segment,type:terms)),params:(addLegend:!t,addTooltip:!t,isDonut:!t,labels:(last_level:!t,show:!t,truncate:100,values:!t),legendPosition:left,type:pie),title:%27New%20Visualization%27,type:pie))',
  //       target: '_new',
  //     },
  //     {
  //       title: 'API Duration Average Last 30 Days',
  //       data: 'apiDurationAverageLast30Days',
  //       url: 'http://localhost:5601/app/kibana#/visualize/create?type=histogram&indexPattern=88ba8cf0-1e68-11e9-af33-f7dbce9cb9e9&_g=(refreshInterval:(display:Off,pause:!f,value:0),time:(from:now-30d,mode:quick,to:now))&_a=(filters:!(),linked:!f,query:(language:lucene,query:%27%27),uiState:(),vis:(aggs:!((enabled:!t,id:%271%27,params:(customLabel:%27Duration%20Average%27,field:DurationMs),schema:metric,type:avg),(enabled:!t,id:%272%27,params:(field:Url.keyword,missingBucket:!f,missingBucketLabel:Missing,order:desc,orderBy:%271%27,otherBucket:!t,otherBucketLabel:Other,size:10),schema:segment,type:terms)),params:(addLegend:!t,addTimeMarker:!f,addTooltip:!t,categoryAxes:!((id:CategoryAxis-1,labels:(filter:!f,show:!t,truncate:100),position:bottom,scale:(type:linear),show:!t,style:(),title:(),type:category)),grid:(categoryLines:!f,style:(color:%23eee)),legendPosition:left,seriesParams:!((data:(id:%271%27,label:%27Duration%20Average%27),drawLinesBetweenPoints:!t,mode:stacked,show:true,showCircles:!t,type:histogram,valueAxis:ValueAxis-1)),times:!(),type:histogram,valueAxes:!((id:ValueAxis-1,labels:(filter:!f,rotate:0,show:!t,truncate:100),name:LeftAxis-1,position:left,scale:(mode:normal,type:linear),show:!t,style:(),title:(text:%27Duration%20Average%27),type:value))),title:%27New%20Visualization%27,type:histogram))',
  //       target: '_new',
  //     },
  //   ],
  // },
  {
    title: 'Help',
    link: '/pages/help',
  }
];
