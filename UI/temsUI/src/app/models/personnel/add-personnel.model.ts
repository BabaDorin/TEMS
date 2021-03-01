interface IAddPersonnel {
    name: string,
    phoneNumber?: string,
    email?: string;
    position?: string;
    // imagePath: string -- TO BE IMPLEMENTED LATER
}

export class AddPersonnel implements IAddPersonnel{
    name: string;
    phoneNumber?: string;
    email?: string;
    position?: string;

    constructor(){
        this.name = 'Baba Doreean';
        this.phoneNumber = '+373454554';
        this.email = 'babadorean@gmail.com.md.ru.it';
        this.position = 'tehnologic';
    }
}