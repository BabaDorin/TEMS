export const API_URL = 'https://localhost:44358';
export const API_PROP_URL = API_URL + '/property';
export const API_EQTYPE_URL = API_URL + '/equipmenttype';
export const API_EQDEF_URL = API_URL + '/equipmentdefinition';
export const API_EQ_URL = API_URL + '/equipment';
export const API_LOG_URL = API_URL + '/log';
export const API_ISU_URL = API_URL + '/ticket';
export const API_ALL_URL = API_URL + '/allocation';
export const API_ROOM_URL = API_URL + '/room';
export const API_PERS_URL = API_URL + '/personnel';
export const API_KEY_URL = API_URL + '/key';
export const API_ANN_URL = API_URL + '/announcement';
export const API_LBR_URL = API_URL + '/library';
export const API_USER_URL = API_URL + '/temsuser';
export const API_ROLE_URL = API_URL + '/userrole';
export const API_AUTH_URL = API_URL + '/auth';
export const API_PROFILE_URL = API_URL + '/profile';
export const API_REP_URL = API_URL + '/report';
export const API_EMAIL_URL = API_URL + '/email';
export const API_ARCH_URL = API_URL + '/archieve';
export const API_SYSCONF_URL = API_URL + '/system';
export const API_ANALYTICS_URL = API_URL + '/analytics';



export interface IEntityCollection{
    roomIds?: string[];
    personnelIds?: string[];
    equipmentIds?: string[];
}