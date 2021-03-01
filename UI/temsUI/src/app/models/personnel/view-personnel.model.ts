interface IViewPersonnel {
    id: string,
    name: string,
    phoneNumber?: string,
    email?: string;
    position?: string;
    // imagePath: string -- TO BE IMPLEMENTED LATER
}

export class ViewPersonnel implements IViewPersonnel{
    id: string;
    name: string;
    phoneNumber?: string;
    email?: string;
    position?: string;

    constructor(){
        this.id = '1',
        this.name = 'Baba Doreean';
        this.phoneNumber = '+373454554';
        this.email = 'babadorean@gmail.com.md.ru.it';
        this.position = 'tehnologic';
    }
}