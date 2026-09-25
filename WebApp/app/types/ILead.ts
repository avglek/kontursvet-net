export interface ILead {
  name: string;
  phone: ILeadPhone;
  home: string;
  location: string;
  message: string;
}

export interface ILeadPhone {
  digital: string;
  format: string;
}

export interface ILeadAttachment {
  filename: string;
  ur: string;
  contentType?: string | undefined;
  encoding: 'base64';
}

export interface ILeadMessage {
  text: ILead;
  attachments: ILeadAttachment[];
}

export interface ILeadForm {
  name: string;
  phoneDigital: string;
  phoneFormat: string;
  home: string;
  location: string;
  message: string;
}
