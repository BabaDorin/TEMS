export interface RecentComment {
    id: number;
    Name: string;
    Time: Date;
    Message: string;
    Image: string
}

export const recentComments: RecentComment[] = [
    {
        id: 1,
        Name: 'Sonu Nigam',
        Time: new Date(),
        Message: `Donec ac condimentum massa. Etiam pellentesque pretium lacus. Phasellus ultricies dictum suscipit.
        Aenean commodo dui pellentesque molestie feugiat`,
        Image:'assets/images/users/1.jpg'
    },
    {
        id: 2,
        Name: 'Loe Caprio',
        Time: new Date(),
        Message: `Donec ac condimentum massa. Etiam pellentesque pretium lacus. Phasellus ultricies dictum suscipit.
        Aenean commodo dui pellentesque molestie feugiat`,
        Image:'assets/images/users/4.jpg'
    },
    {
        id: 1,
        Name: 'Forest Gump',
        Time: new Date(),
        Message: `Donec ac condimentum massa. Etiam pellentesque pretium lacus. Phasellus ultricies dictum suscipit.
        Aenean commodo dui pellentesque molestie feugiat`,
        Image:'assets/images/users/5.jpg'
    }
]