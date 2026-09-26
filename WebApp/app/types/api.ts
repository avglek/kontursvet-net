// ============ Auth ============
export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResult {
  accessToken: string;
  username: string;
  role: string;
}

export interface AdminMe {
  id: number;
  username: string;
  role: string;
}

// ============ Profile / Card ============
export interface PortfolioCardDto {
  id: number;
  link: string;
  title: string;
  subTitle: string;
  description: string;
  img: { src: string; alt: string };
}

export interface PortfolioCardViewDto {
  id: number;
  name: string;
  part: string;
  title: string;
  description: string;
  task: string;
  works: string[];
  location: string;
  term: string;
  team: string;
  period: string;
  features: string;
  meta: string[];
  gallery: GalleryItemDto[];
}

export interface GalleryItemDto {
  key: number;
  src: string;
  alt: string;
  figcaption: string;
}

// ============ Files ============
export interface StoredFile {
  url: string;
  filename: string;
  size: number;
  contentType: string;
}

//========== Lead Attachment =======
export interface LeadTokenAttachment {
  leadId: string; // uid сообщения
  token: string; // токен загруженного файла
  fileName: string;
  contentType: string;
}

// ============ Errors ============
export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
}
