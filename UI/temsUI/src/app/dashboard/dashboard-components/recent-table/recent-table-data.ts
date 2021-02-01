export interface RecentSale {
    id: number;
    Name: string;
    Status: string;
    Date: Date;
    Price: number
}

export const recentSales: RecentSale[] = [
    {
        id: 1,
        Name: 'Elite admin',
        Status: 'SALE',
        Date: new Date('03/18/2017'),
        Price: 24
    },
    {
        id: 2,
        Name: 'Real Homes WP Theme',
        Status: 'EXTENDED',
        Date: new Date('03/20/2017'),
        Price: 1250
    },
    {
        id: 3,
        Name: 'Ample Admin',
        Status: 'EXTENDED',
        Date: new Date('03/25/2017'),
        Price: 1250
    },
    {
        id: 4,
        Name: 'Medical Pro WP Theme',
        Status: 'TAX',
        Date: new Date('03/05/2017'),
        Price: -24
    },
    {
        id: 5,
        Name: 'Hosting press html',
        Status: 'SALE',
        Date: new Date('03/12/2017'),
        Price: 24
    },
    {
        id: 6,
        Name: 'Digital Agency PSD',
        Status: 'SALE',
        Date: new Date('03/27/2017'),
        Price: -14
    },
    {
        id: 7,
        Name: 'Helping Hands WP Theme',
        Status: 'MEMBER',
        Date: new Date('03/31/2017'),
        Price: 64
    }


]